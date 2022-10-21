using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services.Files;

public interface IFileService : IApiService<Guid, GetFileResponse>
{
    Task<Result<GetFileResponse>> GetByHashAsync(string hash, CancellationToken cancellationToken = default);
    Task<Result<Guid>> EditAsync(EditFileRequest editFileRequest, CancellationToken cancellationToken = default);
    Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Guid>> UploadAsync(Stream fileStream, UploadFileRequest uploadFileRequest, CancellationToken cancellationToken = default);
    Task<Result<FileStreamInfo>> DownloadById(Guid id);
    Task<Result<FileStreamInfo>> DownloadByHash(string hash);
}
