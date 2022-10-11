using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.API.Services;

public interface IService<TId, TResult>
{
    Task<Result<IEnumerable<TResult>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<TResult>> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    
}
