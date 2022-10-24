using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiEditClient<TId, TRequest>
{
    Task<Result<TId>> EditAsync(TRequest request, CancellationToken cancellationToken = default);
}