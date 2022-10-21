using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiReadonlyClient<TId>
{
    Task<Result<IEnumerable<TResult>>> GetAllAsync<TResult>(CancellationToken cancellationToken = default);
    Task<Result<TResult>> GetByIdAsync<TResult>(TId id, CancellationToken cancellationToken = default);
    Task<PageResult<TResult>> GetPageAsync<TResult>(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}