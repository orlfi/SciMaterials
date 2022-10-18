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
        // TODO: update current user claims
    }

    public void Delete(Authority authority)
    {
        _authenticationCache.DeleteAuthority(authority.Id);
        // TODO: update current user claims
    }

    public void AddAuthorityToGroup(AuthorityGroup group, Authority authority)
    {
        _authenticationCache.AddAuthorityToGroup(group.Id, group.Name, authority.Id);
    }

    public void AddAuthority(string authorityName)
    {
        _authenticationCache.AddAuthority(authorityName);
    }

    public void RemoveAuthorityFromGroup(AuthorityGroup group, Authority authority)
    {
        _authenticationCache.RemoveAuthorityFromGroup(group.Id, group.Name, authority.Id);
        // TODO: update current user claims
    }

    public bool AuthoritiesExist(string[] authorities)
    {
        return _authenticationCache.AuthoritiesExist(authorities);
    }

    public void AddAuthorityGroup(string authorityName)
    {
        _authenticationCache.AddAuthorityGroup(authorityName);
    }
}