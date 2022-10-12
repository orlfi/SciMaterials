using AutoMapper;
using SciMaterials.DAL.Contexts;
using File = SciMaterials.DAL.Models.File;
using SciMaterials.DAL.Models;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Services.Files;
using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;
using SciMaterials.DAL.UnitOfWork;
using SciMaterials.Contracts.API.Settings;
using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.API.Services;
using System.Reflection.Metadata.Ecma335;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace SciMaterials.Services.API.Services.Files;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly IFileStore _fileStore;
    private readonly IUnitOfWork<SciMaterialsContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly string _path;
    private readonly bool _overwrite;

    public FileService(
        ILogger<FileService> logger,
        IApiSettings apiSettings,
        IFileStore fileStore,
        IUnitOfWork<SciMaterialsContext> unitOfWork,
        IMapper mapper)
    {
        _logger = logger;
        _fileStore = fileStore;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _overwrite = apiSettings.OverwriteFile;
        _path = apiSettings.BasePath;
        if (string.IsNullOrEmpty(_path))
            throw new ArgumentNullException(nameof(apiSettings.BasePath));
    }

    public async Task<Result<IEnumerable<GetFileResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var files = await _unitOfWork.GetRepository<File>().GetAllAsync();
        var result = _mapper.Map<List<GetFileResponse>>(files);
        return result;
    }

    public async Task<Result<GetFileResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if ((await _unitOfWork.GetRepository<File>().GetByIdAsync(id)) is File file)
        {
            return _mapper.Map<GetFileResponse>(file);
        }

        return Result<GetFileResponse>.Error((int)ResultCodes.NotFound, $"File with ID {id} not found");
    }

    public async Task<Result<GetFileResponse>> GetByHashAsync(string hash)
    {
        if ((await _unitOfWork.GetRepository<File>().GetByHashAsync(hash)) is File file)
        {
            return _mapper.Map<GetFileResponse>(file);
        }

        return await Result<GetFileResponse>.ErrorAsync((int)ResultCodes.NotFound, $"File with hash {hash} not found");
    }

    public async Task<Result<FileStreamInfo>> DownloadByHash(string hash)
    {

        if (await GetByHashAsync(hash) is not { } getFileResponse)
            return Result<FileStreamInfo>.Error((int)ResultCodes.NotFound, $"File with hash {hash} not found");

        var readFromPath = Path.Combine(_path, getFileResponse.Data.Id.ToString());
        return new FileStreamInfo(getFileResponse.Data.Name, getFileResponse.Data.ContentTypeName, _fileStore.OpenRead(readFromPath));
    }


    public async Task<Result<FileStreamInfo>> DownloadById(Guid id)
    {

        if (await GetByIdAsync(id) is not { } getFileResponse)
            return Result<FileStreamInfo>.Error((int)ResultCodes.NotFound, $"File with ID {id} not found");

        var readFromPath = Path.Combine(_path, id.ToString());
        return new FileStreamInfo(getFileResponse.Data.Name, getFileResponse.Data.ContentTypeName, _fileStore.OpenRead(readFromPath));
    }

    public async Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.GetRepository<File>().GetByNameAsync(uploadFileRequest.Name) is { } file && !_overwrite)
        {
            _logger.LogInformation("File with name {fileNameWithExension} alredy exist", uploadFileRequest.Name);
            return await Result<Guid>.ErrorAsync((int)ResultCodes.FileAlreadyExist, $"File with name {uploadFileRequest.Name} alredy exist");
        }

        var writeToStoreResult = await WriteToStore(uploadFileRequest, fileStream, cancellationToken);
        if (!writeToStoreResult.Succeeded)
        {
            return await Result<Guid>.ErrorAsync(writeToStoreResult.Code, writeToStoreResult.Messages.First());
        }

        return await WriteToDatabase(writeToStoreResult.Data, cancellationToken);
    }

    private async Task<Result<Guid>> WriteToDatabase(FileMetadata metadata, CancellationToken cancellationToken = default)
    {
        var contentTypeModel = await _unitOfWork.GetRepository<ContentType>().GetByNameAsync(metadata.ContentTypeName);
        if (contentTypeModel is null)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Content type <{metadata.ContentTypeName}> not found.");

        // TODO: Change to AspNet Core User                
        var author = (await _unitOfWork.GetRepository<Author>().GetAllAsync()).First();
        if (author is null)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Author not found.");

        var file = new File
        {
            Id = metadata.Id,
            Name = metadata.Name,
            ContentTypeId = contentTypeModel.Id,
            Hash = metadata.Hash,
            Size = metadata.Size,
            AuthorId = author.Id,
            CreatedAt = DateTime.Now
        };

        await _unitOfWork.GetRepository<File>().AddAsync(file);
        if (await _unitOfWork.SaveContextAsync() <= 0)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");

        return await Result<Guid>.SuccessAsync(file.Id, "File created");
    }


    private async Task<Result<FileMetadata>> WriteToStore(UploadFileRequest uploadFileRequest, Stream fileStream, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = Guid.NewGuid();
            var path = Path.Combine(_path, id.ToString());
            var fileWriteResult = await _fileStore.WriteAsync(path, fileStream, cancellationToken).ConfigureAwait(false);

            var metadata = _mapper.Map<FileMetadata>(uploadFileRequest);
            metadata.Id = id;
            metadata.Size = fileWriteResult.Size;
            metadata.Hash = fileWriteResult.Hash;

            var metadataJsonString = JsonSerializer.Serialize(metadata);
            _ = await _fileStore.WriteAsync(GetMetadataPath(path), metadataJsonString, cancellationToken).ConfigureAwait(false);

            if (await _unitOfWork.GetRepository<File>().GetByHashAsync(metadata.Hash) is { } existingFile)
            {
                string message = $"File with the same hash {existingFile.Hash} already exists with id: {existingFile.Id.ToString()}";
                _fileStore.Delete(path);
                _logger.LogError(message);
                return await Result<FileMetadata>.ErrorAsync((int)ResultCodes.FileAlreadyExist, message);
            }

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when saving a file to storage");
        }
        return await Result<FileMetadata>.ErrorAsync((int)ResultCodes.ApiError, "Error when saving a file to storage");
    }

    // public async Task<Result<Guid>> UploadAsync2(Stream sourceStream, UploadFileRequest uploadFileRequest, CancellationToken cancellationToken = default)
    // {
    //     var fileRepository = _unitOfWork.GetRepository<File>();
    //     var fileNameWithExension = Path.GetFileName(fileName);
    //     var file = await fileRepository.GetByNameAsync(fileNameWithExension);

    //     if (file is not null && !_overwrite)
    //     {
    //         _logger.LogInformation("File with name {fileNameWithExension} alredy exist", fileNameWithExension);
    //         return await Result<Guid>.ErrorAsync((int)ResultCodes.FileAlreadyExist, $"File with name {fileNameWithExension} alredy exist");
    //     }


    //     FileMetadata fileMetadata = new()
    //     {
    //         Id = randomFileName,
    //         Name = fileName,
    //         ContentTypeName = contentType,
    //     };




    //     var saveToPath = Path.Combine(_path, randomFileName.ToString());

    //     try
    //     {
    //         fileMetadata = await _fileStore.WriteAsync(saveToPath, sourceStream, fileMetadata, cancellationToken).ConfigureAwait(false);
    //         if (await fileRepository.GetByHashAsync(fileMetadata.Hash) is File existingFile)
    //         {
    //             string message = $"File with the same hash {existingFile.Hash} already exists with id: {existingFile.Id.ToString()}";
    //             _fileStore.Delete(saveToPath);
    //             _logger.LogInformation(message);
    //             return await Result<Guid>.ErrorAsync((int)ResultCodes.FileAlreadyExist, message);
    //         }

    //         if (file is null)
    //             return await AddAsync(fileMetadata, cancellationToken);

    //         file.Hash = fileMetadata.Hash;
    //         file.Size = fileMetadata.Size;
    //         return await OverrideAsync(file, cancellationToken);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "File save error.");
    //         return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "File save error.");
    //     }
    // }

    public async Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if ((await DeleteFileFromFileSystem(id, cancellationToken)) is { Succeeded: false } deleteFromFileSystemResult)
        {
            return deleteFromFileSystemResult;
        }

        return await DeleteFileFromDatabase(id);
    }

    private async Task<Result<Guid>> DeleteFileFromFileSystem(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deletePath = Path.Combine(_path, id.ToString());
            _fileStore.Delete(deletePath);

            return await Result<Guid>.SuccessAsync(id, $"File with ID {id} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when deleting a file  with ID {id} from storage", id);
        }

        return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, $"Error when deleting a file with ID {id} from storage.");
    }

    private async Task<Result<Guid>> DeleteFileFromDatabase(Guid id, CancellationToken cancellationToken = default)
    {
        var fileRepository = _unitOfWork.GetRepository<File>();

        if (await fileRepository.GetByIdAsync(id) is File file)
        {
            await fileRepository.DeleteAsync(file);
            await _unitOfWork.SaveContextAsync();
        }

        return await Result<Guid>.SuccessAsync($"File with ID {id} deleted");
    }


    private async Task<Result<Guid>> OverrideAsync(File file, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepository<File>().UpdateAsync(file);

        if (await _unitOfWork.SaveContextAsync() <= 0)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");

        return await Result<Guid>.SuccessAsync(file.Id, "File overrided");
    }

    private static string GetMetadataPath(string filePath)
    => Path.ChangeExtension(filePath, "json");
}
