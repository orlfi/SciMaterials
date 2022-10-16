using Microsoft.AspNetCore.Components.Authorization;

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
}