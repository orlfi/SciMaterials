using Microsoft.AspNetCore.Components.Authorization;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public class TestAccountsService : IAccountsService
{
    private readonly AuthenticationCache _authenticationCache;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public TestAccountsService(AuthenticationCache authenticationCache, AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationCache = authenticationCache;
        _authenticationStateProvider = authenticationStateProvider;
    }

    private TestAuthenticationStateProvider ActualProvider => (TestAuthenticationStateProvider)_authenticationStateProvider;

    public List<AuthorityGroup> AuthorityGroupsList()
    {
        // take view models
        return _authenticationCache.AuthorityGroupsList();
    }

    public List<UserInfo> UsersList()
    {
        // take view models
        return _authenticationCache.UsersList();
    }

    public async Task ChangeAuthority(Guid userId, Guid authorityId)
    {
        var result = _authenticationCache.ChangeAuthorityGroup(userId, authorityId);
        
        if (!result.Succeeded)
        {
            // TODO: handle failure
            return;
        }
        if(await ActualProvider.TakeCurrentUserId() != userId) return;
        await ActualProvider.UpdateUserData();
    }

    public async Task Delete(Guid userId)
    {
        _authenticationCache.DeleteUser(userId);

        if (await ActualProvider.TakeCurrentUserId() != userId) return;
        await ActualProvider.UpdateUserData();
    }
}