using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public class TestAuthenticationService : IAuthenticationService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly AuthenticationCache _authenticationCache;

    public TestAuthenticationService(
        ILocalStorageService localStorageService,
        AuthenticationStateProvider authenticationStateProvider,
        AuthenticationCache authenticationCache)
    {
        _localStorageService = localStorageService;
        _authenticationStateProvider = authenticationStateProvider;
        _authenticationCache = authenticationCache;
    }

    public Task<bool> SignUp(SignUpForm formData)
    {
        return Task.FromResult(_authenticationCache.TryAdd(formData.Email!, formData.Password!, formData.Username!));
    }

    public async Task SignIn(SignInForm formData)
    {
        if (!_authenticationCache.TryGetIdentity(formData.Email!, formData.Password!, out var identity, out var userId)) return;

        await _localStorageService.SetItemAsStringAsync("authToken", userId.ToString());

        ((TestAuthenticationStateProvider)_authenticationStateProvider).NotifyUserSignIn(identity);
    }

    public async Task Logout()
    {
        await _localStorageService.RemoveItemAsync("authToken");
        ((TestAuthenticationStateProvider)_authenticationStateProvider).NotifyUserLogout();
    }
}