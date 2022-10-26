using System.Text.Json;

using SciMaterials.Contracts.API.DTO.AuthRoles;
using SciMaterials.Contracts.API.DTO.Clients;
using SciMaterials.Contracts.API.Services.Identity;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityRolesService : IRolesService
{
    private readonly IIdentityRolesClient<IdentityClientResponse, AuthRoleRequest> _rolesClient;
    private readonly IAuthenticationService _authenticationService;

    public IdentityRolesService(
        IIdentityRolesClient<IdentityClientResponse, AuthRoleRequest> rolesClient,
        IAuthenticationService authenticationService)
    {
        _rolesClient = rolesClient;
        _authenticationService = authenticationService;
    }

    public async Task<IReadOnlyList<UserRole>> RolesList()
    {
        var response = await _rolesClient.GetAllRolesAsync(CancellationToken.None);
        if (!response.Succeeded) return Array.Empty<UserRole>();

        var roles = JsonSerializer.Deserialize<IReadOnlyList<UserRole>>(response.Content);
        return roles ?? Array.Empty<UserRole>();
    }

    public async Task<bool> AddRole(string roleName)
    {
        var response = await _rolesClient.CreateRoleAsync(new AuthRoleRequest() { RoleName = roleName });
        if (response.Succeeded) return true;

        // TODO: handle failure
        return false;

    }

    public async Task<bool> DeleteRole(string roleId)
    {
        var response = await _rolesClient.DeleteRoleByIdAsync(new AuthRoleRequest() { RoleId = roleId });
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return false;
        }

        // Validate that current user in role
        // Refresh if it is

        return true;
    }
}