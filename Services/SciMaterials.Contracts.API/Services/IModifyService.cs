using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services;

public interface IModifyService<TId>
{
    Task<Result<TId>> AddAsync<TModel>(TModel model, CancellationToken cancellationToken = default);
    Task<Result<TId>> EditAsync<TModel>(TModel model, CancellationToken cancellationToken = default);
    Task<Result<TId>> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}
