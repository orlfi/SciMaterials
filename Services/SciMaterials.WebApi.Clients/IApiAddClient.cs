using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;

namespace SciMaterials.WebApi.Clients;

public interface IApiAddClient<TRequest, TId>
{
    Task<Result<TId>> AddAsync(TRequest model, CancellationToken cancellationToken = default);
}