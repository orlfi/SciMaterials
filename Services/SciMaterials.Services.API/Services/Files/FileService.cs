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
        var file = await _unitOfWork.GetRepository<File>().GetByIdAsync(id);

        if (file is null)
            return Result<GetFileResponse>.Error((int)ResultCodes.NotFound, $"File with ID {id} not found");

        var result = _mapper.Map<GetFileResponse>(file);
        return result;
    }

    public async Task<Result<GetFileResponse>> GetByHashAsync(string hash)
    {
        var file = await _unitOfWork.GetRepository<File>().GetByHashAsync(hash);

        if (file is null)
            return await Result<GetFileResponse>.ErrorAsync((int)ResultCodes.NotFound, $"File with hash {hash} not found");

        var result = _mapper.Map<GetFileResponse>(file);
        return result;
    }

    public Stream GetFileStream(Guid id)
    {
        var readFromPath = Path.Combine(_path, id.ToString());
        return _fileStore.OpenRead(readFromPath);
    }

    public async Task<Result<Guid>> UploadAsync(Stream sourceStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var fileRepository = _unitOfWork.GetRepository<File>();
        var fileNameWithExension = Path.GetFileName(fileName);
        var file = await fileRepository.GetByNameAsync(fileNameWithExension);

        if (file is not null && !_overwrite)
        {
            _logger.LogInformation("File with name {fileNameWithExension} alredy exist", fileNameWithExension);
            return await Result<Guid>.ErrorAsync((int)ResultCodes.FileAlreadyExist, $"File with name {fileNameWithExension} alredy exist");
        }

        var contentTypeModel = await _unitOfWork.GetRepository<ContentType>().GetByNameAsync(contentType);
        if (contentTypeModel is null)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Content type <{contentType}> not found.");

        // TODO: Change to AspNet Core User                
        var author = (await _unitOfWork.GetRepository<Author>().GetAllAsync()).First();
        if (author is null)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Author not found.");

        FileMetadata fileMetadata = new()
        {
            Id = Guid.NewGuid(),
            Name = fileName,
            ContentTypeName = contentType,
        };

        var randomFileName = Guid.NewGuid();
        var saveToPath = Path.Combine(_path, randomFileName.ToString());
        var metadataPath = Path.Combine(_path, randomFileName + ".json");

        try
        {
            fileMetadata = await _fileStore.WriteAsync(saveToPath, sourceStream, fileMetadata, cancellationToken).ConfigureAwait(false);
            if (await fileRepository.GetByHashAsync(fileMetadata.Hash) is File existingFile)
            {
                string message = $"File with the same hash {existingFile.Hash} already exists with id: {existingFile.Id.ToString()}";
                _fileStore.Delete(saveToPath);
                _logger.LogInformation(message);
                return await Result<Guid>.ErrorAsync((int)ResultCodes.FileAlreadyExist, message);
            }

            if (file is null)
                return await AddAsync(fileMetadata, cancellationToken);

            file.Hash = fileMetadata.Hash;
            file.Size = fileMetadata.Size;
            return await OverrideAsync(file, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File save error.");
            return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "File save error.");
        }
    }

    private async Task<Result<Guid>> AddAsync(FileMetadata fileMetadata, CancellationToken cancellationToken = default)
    {
        var contentTypeModel = await _unitOfWork.GetRepository<ContentType>().GetByNameAsync(fileMetadata.ContentTypeName);
        if (contentTypeModel is null)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Content type <{fileMetadata.ContentTypeName}> not found.");

        // TODO: Change to AspNet Core User                
        var author = (await _unitOfWork.GetRepository<Author>().GetAllAsync()).First();
        if (author is null)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.NotFound, $"Author not found.");

        var file = new File
        {
            Id = fileMetadata.Id.Value,
            Name = fileMetadata.Name,
            ContentTypeId = contentTypeModel.Id,
            Hash = fileMetadata.Hash,
            Size = fileMetadata.Size,
            AuthorId = author.Id,
            CreatedAt = DateTime.Now
        };

        await _unitOfWork.GetRepository<File>().AddAsync(file);
        if (await _unitOfWork.SaveContextAsync() <= 0)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");

        return await Result<Guid>.SuccessAsync(file.Id, "File created");
    }

    private async Task<Result<Guid>> OverrideAsync(File file, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.GetRepository<File>().UpdateAsync(file);

        if (await _unitOfWork.SaveContextAsync() <= 0)
            return await Result<Guid>.ErrorAsync((int)ResultCodes.ServerError, "Save context error");

        return await Result<Guid>.SuccessAsync(file.Id, "File overrided");
    }
}
