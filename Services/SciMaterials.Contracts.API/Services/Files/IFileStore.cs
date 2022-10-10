using SciMaterials.Contracts.API.Models;

namespace SciMaterials.Contracts.API.Services.Files;

public interface IFileStore
{
    Task<FileSaveResult> WriteAsync(string path, Stream stream, CancellationToken cancellationToken = default);
    Task WriteMetadataAsync<T>(string path, T data, CancellationToken cancellationToken = default);
    Stream OpenRead(string path);
    Task<T> ReadMetadataAsync<T>(string path, CancellationToken cancellationToken = default);
    void Delete(string path);
}