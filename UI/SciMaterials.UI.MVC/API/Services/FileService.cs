using System.Diagnostics;
using SciMaterials.UI.MVC.API.Exceptions;
using SciMaterials.UI.MVC.API.Models;
using SciMaterials.UI.MVC.API.Data.Interfaces;
using SciMaterials.UI.MVC.API.Services.Interfaces;
using SciMaterials.UI.MVC.API.Configuration.Interfaces;
using SciMaterials.Domain.Core;

namespace SciMaterials.UI.MVC.Services;

public class FileService : IFileService<Guid>
{
    private readonly ILogger<FileService> _logger;
    private readonly IFileRepository<Guid> _fileRepository;
    private readonly IFileStore _fileStore;
    private readonly string _path;
    private readonly bool _overwrite;

    public FileService(ILogger<FileService> logger, IApiSettings apiSettings, IFileRepository<Guid> fileRepository, IFileStore fileStore)
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _fileStore = fileStore;
        _overwrite = apiSettings.OverwriteFile;
        _path = apiSettings.BasePath;
        if (string.IsNullOrEmpty(_path))
            throw new ArgumentNullException(nameof(apiSettings.BasePath));
    }

    public Result<IEnumerable<FileModel>> GetAll()
    {
        var result = _fileRepository.GetAll().ToList();
        return result;
    }

    public Result<FileModel> GetById(Guid id)
    {
        var model = _fileRepository.GetById(id);

        if (model is null)
            throw new FileNotFoundException($"File with id {id} not found");

        return model;
    }

    public Result<FileModel> GetByHash(string hash)
    {
        var model = _fileRepository.GetByHash(hash);

        if (model is null)
            throw new FileNotFoundException($"File with hash {hash} not found");

        return model;
    }
    public Stream GetFileStream(Guid id)
    {
        var readFromPath = Path.Combine(_path, id.ToString());
        return _fileStore.OpenRead(readFromPath);
    }

    public async Task<Result<FileModel>> UploadAsync(Stream sourceStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var fileNameWithExension = Path.GetFileName(fileName);
        var fileModel = _fileRepository.GetByName(fileNameWithExension);

        if (fileModel is not null && !_overwrite)
        {
            var exception = new FileAlreadyExistException(fileName);
            _logger.LogError(exception, null);
            throw exception;
        }

        var randomFileName = fileModel?.Id ?? Guid.NewGuid();
        var saveToPath = Path.Combine(_path, randomFileName.ToString());
        var metadataPath = Path.Combine(_path, randomFileName + ".json");


        Stopwatch sw = new Stopwatch();
        sw.Start();
        var saveResult = await _fileStore.WriteAsync(saveToPath, sourceStream, cancellationToken).ConfigureAwait(false);
        sw.Stop();
        _logger.LogInformation("Ellapsed:{ellapsed} сек", sw.Elapsed.TotalSeconds);

        if (CheckFileExistByHash(saveResult.Hash, out var existingFileInfo) && existingFileInfo is not null)
        {
            var exception = new FileAlreadyExistException(fileName, $"File with the same hash {existingFileInfo.Hash} already exists with id: {existingFileInfo.Id.ToString()}");
            _fileStore.Delete(saveToPath);
            _logger.LogError(exception, null);
            throw exception;
        }

        if (fileModel is null)
        {
            fileModel = new FileModel
            {
                Id = randomFileName,
                FileName = fileNameWithExension,
                ContentType = contentType,
                Hash = saveResult.Hash,
                Size = saveResult.Size
            };
        }
        else
        {
            fileModel.Hash = saveResult.Hash;
            fileModel.Size = saveResult.Size;
        }
        _fileRepository.AddOrUpdate(fileModel);

        await _fileStore.WriteMetadataAsync(metadataPath, fileModel, cancellationToken).ConfigureAwait(false);
        return fileModel;
    }

    private bool CheckFileExistByHash(string hash, out FileModel? fileInfo)
    {
        fileInfo = _fileRepository.GetByHash(hash);
        return fileInfo is not null;
    }
}
