using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Responses.Roles;

namespace SciMaterials.Contracts.Identity.API;

public interface IRolesClient
{
    Task<ClientCreateRoleResponse> CreateRoleAsync(CreateRoleRequest CreateRoleRequest, CancellationToken Cancel = default);
    
    Task<ClientGetAllRolesResponse> GetAllRolesAsync(CancellationToken Cancel = default);
    
    Task<ClientGetRoleByIdResponse> GetRoleByIdAsync(string RoleId, CancellationToken CancellationToken = default);
    
    Task<ClientEditRoleNameByIdResponse> EditRoleNameByIdAsync(EditRoleNameByIdRequest EditRoleRequest, CancellationToken CancellationToken = default);
    
    Task<ClientDeleteRoleByIdResponse> DeleteRoleByIdAsync(string RoleId, CancellationToken CancellationToken = default);
    
    Task<ClientAddRoleToUserResponse> AddRoleToUserAsync(AddRoleToUserRequest AddRoleRequest, CancellationToken CancellationToken = default);
    
    Task<ClientDeleteUserRoleByEmailResponse> DeleteUserRoleByEmailAsync(string Email, string RoleName, CancellationToken CancellationToken = default);
    
    Task<ClientGetAllUserRolesByEmailResponse> GetAllUserRolesByEmailAsync(string Email, CancellationToken CancellationToken = default);
}