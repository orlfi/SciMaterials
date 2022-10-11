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

        var userData = JsonSerializer.Deserialize<UserInfo>(token);
        if (userData is null) return Anonymous;

        return new(new(new ClaimsIdentity(new []
        {
            new Claim(ClaimTypes.Name, userData.Nick),
            new Claim(ClaimTypes.Email, userData.Email),
            new Claim(ClaimTypes.Role, "User")
        }, "Some Auth Policy Type")));
    }

    public void NotifyUserSignIn(ClaimsIdentity userClaims)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new(userClaims))));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    public record UserInfo(string Nick, string Email);
}