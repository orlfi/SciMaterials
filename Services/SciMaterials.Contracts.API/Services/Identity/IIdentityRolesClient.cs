namespace SciMaterials.Contracts.API.Services.Identity;

public interface IIdentityRolesClient<TResponse, TRequest>
{
    Task<TResponse> CreateRoleAsync(TRequest roleRequest, CancellationToken cancellationToken = default);
    
    Task<TResponse> GetAllRolesAsync(CancellationToken cancellationToken = default);
    
    Task<TResponse> GetRoleByIdAsync(TRequest roleRequest, CancellationToken cancellationToken = default);
    
    Task<TResponse> EditRoleByIdAsync(TRequest roleRequest, CancellationToken cancellationToken = default);
    
    Task<TResponse> DeleteRoleByIdAsync(TRequest roleRequest, CancellationToken cancellationToken = default);
    
    Task<TResponse> AddRoleToUserAsync(TRequest roleRequest, CancellationToken cancellationToken = default);
    
    Task<TResponse> DeleteUserRoleByEmailAsync(TRequest roleRequest, CancellationToken cancellationToken = default);
    
    Task<TResponse> ListOfUserRolesAsync(TRequest roleRequest, CancellationToken cancellationToken = default);
}