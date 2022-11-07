using SciMaterials.Contracts.Identity.Clients.Clients;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityRolesService : IRolesService
{
    private readonly IRolesClient _rolesClient;
    private readonly IAuthenticationService _authenticationService;

    public IdentityRolesService(
        IRolesClient rolesClient,
        IAuthenticationService authenticationService)
    {
        _rolesClient = rolesClient;
        _authenticationService = authenticationService;
    }

    public async Task<IReadOnlyList<UserRole>> RolesList()
    {
        var response = await _rolesClient.GetAllRolesAsync(CancellationToken.None);
        if (!response.Succeeded) return new List<UserRole>();
        
        return response.Roles.Select(x=>new UserRole{Id = x.Id, Name = x.RoleName}).ToList();
    }

    public async Task<bool> AddRole(string roleName)
    {
        var response = await _rolesClient.CreateRoleAsync(new() { RoleName = roleName });
        if (response.Succeeded) return true;

        // TODO: handle failure
        return false;

    }

    public async Task<bool> DeleteRole(string roleId)
    {
        var response = await _rolesClient.DeleteRoleByIdAsync(roleId);
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return false;
        }

        // Validate that current user in role
        // Refresh if it is
        await _authenticationService.RefreshCurrentUser();

        return true;
    }
}