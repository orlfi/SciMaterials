using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;

namespace SciMaterials.WebApi.Clients.Files;

public interface IFilesClient
{
    Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken cancellationToken = default);
}