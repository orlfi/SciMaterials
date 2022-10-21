using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiAddClient<TId>
{
    Task<Result<TId>> AddAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default);
}