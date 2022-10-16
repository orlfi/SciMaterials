using Microsoft.AspNetCore.Components.Authorization;
using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public class TestAuthoritiesService : IAuthoritiesService
{
    private readonly AuthenticationCache _authenticationCache;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public TestAuthoritiesService(AuthenticationCache authenticationCache, AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationCache = authenticationCache;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public List<AuthorityGroup> AuthoritiesGroupsList()
    {
        return _authenticationCache.AuthorityGroupsList();
    }

    public List<Authority> AuthoritiesList()
    {
        return _authenticationCache.AuthoritiesList();
    }

    public void Delete(AuthorityGroup authorityGroup)
    {
        _authenticationCache.DeleteAuthorityGroup(authorityGroup.Id, authorityGroup.Name);
    }

    public void Delete(Authority authority)
    {
        _authenticationCache.DeleteAuthority(authority.Id);
    }
}