namespace SciMaterials.Contracts.API.Services.Identity;

public interface IIdentityUserClient<TResponse, TRequest>
{
    Task<TResponse> RegisterUserAsync(TRequest registerUser, CancellationToken cancellationToken = default);
    
    Task<TResponse> LoginUserAsync(TRequest loginUser, CancellationToken cancellationToken = default);
    
    Task<TResponse> LogoutUserAsync(CancellationToken cancellationToken = default);
    
    Task<TResponse> CreateUserAsync(TRequest userRequest, CancellationToken cancellationToken = default);
    
    Task<TResponse> GetUserByEmailAsync(TRequest userRequest, CancellationToken cancellationToken = default);
    
    Task<TResponse> GetAllUsersAsync(CancellationToken cancellationToken = default);

    Task<TResponse> DeleteUserByEmail(TRequest userRequest, CancellationToken cancellationToken = default);
    
    Task<TResponse> DeleteUsersWithOutConfirm(CancellationToken cancellationToken = default);
}