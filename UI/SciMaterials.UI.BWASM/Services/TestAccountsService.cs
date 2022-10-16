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

    public void ChangeAuthority(Guid userId, Guid authorityId)
    {
        // check that current user can change authority
        var result = _authenticationCache.ChangeAuthorityGroup(userId, authorityId);
        // handle result
    }

    public void Delete(Guid userId)
    {
        // check that actual user and user to delete is not same
        _authenticationCache.DeleteUser(userId);
    }
}