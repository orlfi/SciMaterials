using SciMaterials.UI.MVC.API.Models;

namespace SciMaterials.UI.MVC.API.Services.Interfaces;

public interface IFileService<T>
{
    IEnumerable<FileModel> GetAll();
    FileModel GetById(T id);
    FileModel GetByHash(string hash);
    Task<FileModel> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Stream GetFileStream(T id);
}
