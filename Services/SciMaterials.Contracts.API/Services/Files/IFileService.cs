using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.API.Models;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services.Files;

public interface IFileService : IService<Guid, GetFileResponse>
{
    Task<Result<GetFileResponse>> GetByHashAsync(string hash);
    Task<Result<Guid>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Guid>> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    GetFileStreamModel GetFileStream(Guid id);
}
