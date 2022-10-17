using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;

namespace SciMaterials.WebApi.Clients.Files;

public interface IFilesClient : IApiClient<Guid, GetFileResponse>
{
    Task<Result<Guid>> EditAsync(EditFileRequest request, CancellationToken cancellationToken = default);
    Task<Result<GetFileResponse>> GetByHashIdAsync(string hash, CancellationToken cancellationToken = default);
    Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken cancellationToken = default);
}