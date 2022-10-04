using SciMaterials.Domain.Core;
using SciMaterials.UI.MVC.API.Models;

namespace SciMaterials.UI.MVC.API.Services.Interfaces;

public interface IFileService<T>
{
    Result<IEnumerable<FileModel>> GetAll();
    Result<FileModel> GetById(T id);
    Result<FileModel> GetByHash(string hash);
    Task<Result<FileModel>> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Stream GetFileStream(T id);
}
