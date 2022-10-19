using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.Result;

namespace SciMaterials.WebApi.Clients.Files;

public interface IFilesClient :
    IApiReadonlyClient<Guid, GetFileResponse>,
    IApiEditClient<EditFileRequest, Guid>,
    IApiDeleteClient<Guid>
{
    // Task<Result<Guid>> EditAsync(EditFileRequest request, CancellationToken cancellationToken = default);
    Task<Result<FileStreamInfo>> DownloadByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken cancellationToken = default);
    Task<Result<GetFileResponse>> GetByHashIdAsync(string hash, CancellationToken cancellationToken = default);
    Task<Result<FileStreamInfo>> DownloadByHashAsync(string hash, CancellationToken cancellationToken = default);

}