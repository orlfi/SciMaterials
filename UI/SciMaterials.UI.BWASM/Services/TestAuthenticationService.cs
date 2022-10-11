using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.UI.BWASM.Models;

namespace SciMaterials.UI.BWASM.Services;

public class TestAuthenticationService : IAuthenticationService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private static ConcurrentDictionary<string, SignUpForm> _users = new();

    public TestAuthenticationService(ILocalStorageService localStorageService, AuthenticationStateProvider authenticationStateProvider)
    {
        _localStorageService = localStorageService;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task SignUp(SignUpForm formData)
    {
        if (!_users.TryAdd(formData.Email, formData)) return;

        Claim[] userClaims = {
            new(ClaimTypes.Name, formData.Username),
            new(ClaimTypes.Email, formData.Email)
        };

        await SetUserSignIn(userClaims);
    }

    public async Task SignIn(SignInForm formData)
    {
        if (!_users.TryGetValue(formData.Email, out var form) || form.Password != formData.Password) return;

        Claim[] userClaims = {
            new(ClaimTypes.Name, form.Username),
            new(ClaimTypes.Email, formData.Email)
        };

        await SetUserSignIn(userClaims);
    }

    public async Task Logout()
    {
        await _localStorageService.RemoveItemAsync("authToken");
        ((TestAuthenticationStateProvider)_authenticationStateProvider).NotifyUserLogout();
    }

    private async Task SetUserSignIn(Claim[] userClaims)
    {
        ClaimsIdentity userData = new(userClaims);
        await _localStorageService.SetItemAsStringAsync("authToken", JsonSerializer.Serialize(userData));

        ((TestAuthenticationStateProvider)_authenticationStateProvider).NotifyUserSignIn(userData);
    }
}