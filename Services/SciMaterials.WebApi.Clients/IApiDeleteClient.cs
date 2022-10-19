using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;

namespace SciMaterials.WebApi.Clients;

public interface IApiDeleteClient<TId>
{
    Task<Result<TId>> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}