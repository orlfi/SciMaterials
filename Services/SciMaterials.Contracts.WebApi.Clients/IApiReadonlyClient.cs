using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiReadonlyClient<TId, TResult>
{
    Task<Result<IEnumerable<TResult>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<TResult>> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<PageResult<TResult>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}