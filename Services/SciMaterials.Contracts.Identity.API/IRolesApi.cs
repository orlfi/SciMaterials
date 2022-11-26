using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.Identity.API;

public interface IRolesApi
{
    Task<Result.Result> CreateRoleAsync(CreateRoleRequest CreateRoleRequest, CancellationToken Cancel = default);
    
    Task<Result<IEnumerable<AuthRole>>> GetAllRolesAsync(CancellationToken Cancel = default);
    
    Task<Result<AuthRole>> GetRoleByIdAsync(string RoleId, CancellationToken CancellationToken = default);
    
    Task<Result.Result> EditRoleNameByIdAsync(EditRoleNameByIdRequest EditRoleRequest, CancellationToken CancellationToken = default);
    
    Task<Result.Result> DeleteRoleByIdAsync(string RoleId, CancellationToken CancellationToken = default);
    
    Task<Result.Result> AddRoleToUserAsync(AddRoleToUserRequest AddRoleRequest, CancellationToken CancellationToken = default);
    
    Task<Result.Result> DeleteUserRoleByEmailAsync(string Email, string RoleName, CancellationToken CancellationToken = default);
    
    Task<Result<IEnumerable<AuthRole>>> GetUserRolesAsync(string Email, CancellationToken CancellationToken = default);
}