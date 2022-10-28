using System.Text.Json;

using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.Contracts.API.DTO.AuthRoles;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.API.DTO.Clients;
using SciMaterials.Contracts.API.Services.Identity;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityAccountsService : IAccountsService
{
    private readonly IIdentityUserClient<IdentityClientResponse, AuthUserRequest> _usersClient;
    private readonly IIdentityRolesClient<IdentityClientResponse, AuthRoleRequest> _rolesClient;
    private readonly IAuthenticationService _authenticationService;

    public IdentityAccountsService(
        IIdentityUserClient<IdentityClientResponse, AuthUserRequest> usersClient, 
        IIdentityRolesClient<IdentityClientResponse, AuthRoleRequest> rolesClient,
        IAuthenticationService authenticationService)
    {
        _usersClient = usersClient;
        _rolesClient = rolesClient;
        _authenticationService = authenticationService;
    }

    public async Task<IReadOnlyList<UserInfo>> UsersList()
    {
        var response = await _usersClient.GetAllUsersAsync(CancellationToken.None);
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return Array.Empty<UserInfo>();
        }

        var data = JsonSerializer.Deserialize<IEnumerable<User>>(response.Content);
        if (data is null) return Array.Empty<UserInfo>();
        return data.Select(x=>new UserInfo()
        {
            Id = Guid.Parse(x.Id),
            UserName = x.UserName,
            Email = x.Email
        }).ToList();
    }

    public async Task ChangeAuthority(string userEmail, string authorityName)
    {
        _rolesClient.DeleteUserRoleByEmailAsync(
            new AuthRoleRequest()
            {
                Email = userEmail,
                RoleName = authorityName
            },
            CancellationToken.None);
        _rolesClient.AddRoleToUserAsync(new AuthRoleRequest() { Email = userEmail, RoleName = authorityName }, CancellationToken.None);

        if(await _authenticationService.IsCurrentUser(userEmail))
            await _authenticationService.RefreshCurrentUser();
    }

    public async Task Delete(string userEmail)
    {
        var response = await _usersClient.DeleteUserByEmail(new AuthUserRequest() { Email = userEmail }, CancellationToken.None);
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return;
        }

        if (await _authenticationService.IsCurrentUser(userEmail))
            await _authenticationService.RefreshCurrentUser();
        
    }

    private record User(string Id, string Email, string UserName);
}