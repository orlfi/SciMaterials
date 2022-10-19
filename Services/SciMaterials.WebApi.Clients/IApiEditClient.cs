using SciMaterials.Contracts.Result;

namespace SciMaterials.WebApi.Clients;

public interface IApiEditClient<TRequest, TId>
{
    Task<Result<TId>> EditAsync(TRequest request, CancellationToken cancellationToken = default);
}