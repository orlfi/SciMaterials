using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace SciMaterials.UI.BWASM.Services;

public class TestAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private static readonly AuthenticationState Anonymous;

    static TestAuthenticationStateProvider() => Anonymous = new(new(new ClaimsIdentity()));

    public TestAuthenticationStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorageService.GetItemAsync<string>("authToken");
        if (string.IsNullOrWhiteSpace(token)) return Anonymous;

        var userData = JsonSerializer.Deserialize<ClaimsIdentity>(token);
        if (userData is null) return Anonymous;

        return new(new(userData));
    }

    public void NotifyUserSignIn(ClaimsIdentity userClaims)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new(userClaims))));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}