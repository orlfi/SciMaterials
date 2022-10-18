using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;

namespace SciMaterials.WebApi.Clients;

public interface IApiClient<TId, TResult>
{
    Task<Result<IEnumerable<GetFileResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<TResult>> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}