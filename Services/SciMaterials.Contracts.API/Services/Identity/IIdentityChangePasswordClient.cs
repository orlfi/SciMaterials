namespace SciMaterials.Contracts.API.Services.Identity;

public interface IIdentityChangePasswordClient<TResponse, TRequest>
{
    Task<TResponse> ChangePasswordAsync(TRequest passwordRequest, CancellationToken cancellationToken = default);
}