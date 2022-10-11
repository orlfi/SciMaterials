using SciMaterials.Contracts.API.DTO.Files;

namespace SciMaterials.Contracts.API.Services.Files;

public interface IFileStore
{
    Task<FileMetadata> WriteAsync(string path, Stream stream, FileMetadata metadata, CancellationToken cancellationToken = default);
    Stream OpenRead(string path);
    Task<T> ReadMetadataAsync<T>(string path, CancellationToken cancellationToken = default);
    void Delete(string path);
}