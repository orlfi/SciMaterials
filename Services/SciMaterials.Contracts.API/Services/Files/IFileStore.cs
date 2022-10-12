using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Models;

namespace SciMaterials.Contracts.API.Services.Files;

public interface IFileStore
{
    Task<WriteFileResult> WriteAsync(string path, Stream stream, CancellationToken cancellationToken = default);
    Task<WriteFileResult> WriteAsync(string path, string text, CancellationToken cancellationToken = default);
    Stream OpenRead(string path);
    // Task<T> ReadMetadataAsync<T>(string path, CancellationToken cancellationToken = default);
    void Delete(string path);
}