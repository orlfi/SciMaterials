namespace SciMaterials.Contracts.API.Services.Identity;

public interface IIdentityEditUserClient<TResponse, TRequest>
{
    Task<TResponse> EditUserNameByEmailAsync(TRequest editUserRequest, CancellationToken cancellationToken = default);
}