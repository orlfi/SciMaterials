using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services;

public interface IModifyService<TId, TModel>
{
    Task<Result<TId>> AddEditAsync(TModel model, CancellationToken cancellationToken = default);
    Task<Result<TId>> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}
