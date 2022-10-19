using SciMaterials.Contracts.Result;

namespace SciMaterials.WebApi.Clients;

public interface IApiAddClient<TRequest, TId>
{
    Task<Result<TId>> AddAsync(TRequest request, CancellationToken cancellationToken = default);
}