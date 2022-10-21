using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services;

public interface IApiService<TId, TResult>
{
    Task<Result<IEnumerable<TResult>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PageResult<TResult>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<TResult>> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}
