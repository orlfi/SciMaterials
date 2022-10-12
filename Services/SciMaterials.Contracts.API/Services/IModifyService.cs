using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services;

public interface IModifyService<TAddModel, TEditModel, TId>
    where TAddModel : class
    where TEditModel : class
{
    Task<Result<TId>> AddAsync(TAddModel model, CancellationToken cancellationToken = default);
    Task<Result<TId>> EditAsync(TEditModel model, CancellationToken cancellationToken = default);
    Task<Result<TId>> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}
