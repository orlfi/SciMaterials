using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiAddClient<TId, TRequest>
{
    Task<Result<TId>> AddAsync(TRequest request, CancellationToken cancellationToken = default);
}