using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services.Files;

public interface IFileService : IService<Guid, GetFileResponse>
{
    Task<Result<GetFileResponse>> GetByHashAsync(string hash);
    Task<Result<Guid>> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Stream GetFileStream(Guid id);
}
