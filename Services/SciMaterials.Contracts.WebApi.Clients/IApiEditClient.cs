using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiEditClient<TId>
{
    Task<Result<TId>> EditAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default);
}