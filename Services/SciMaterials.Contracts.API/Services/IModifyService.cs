using SciMaterials.Contracts.Result;
using SciMaterials.UI.MVC.API.Models;

namespace SciMaterials.Contracts.API.Services;

public interface IModifyService<TId, TModel>
{
    Task<Result<TId>> AddEditAsync(TModel model, CancellationToken cancellationToken = default);
    Task<Result<TId>> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}
