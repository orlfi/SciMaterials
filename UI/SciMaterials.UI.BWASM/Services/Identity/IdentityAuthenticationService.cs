using System.Security.Claims;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.UI.BWASM.Models;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.API.DTO.Clients;
using SciMaterials.Contracts.API.Services.Identity;
using SciMaterials.UI.BWASM.Utils;

namespace SciMaterials.UI.BWASM.Services.Identity;

public class IdentityAuthenticationService : IAuthenticationService
{
    private readonly IIdentityUserClient<IdentityClientResponse, AuthUserRequest> _client;
    private readonly ILocalStorageService _localStorageService;
    private readonly IdentityAuthenticationStateProvider _authenticationStateProvider;

    public IdentityAuthenticationService(
        IIdentityUserClient<IdentityClientResponse, AuthUserRequest> client,
        ILocalStorageService localStorageService,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _client = client;
        _localStorageService = localStorageService;
        _authenticationStateProvider = (IdentityAuthenticationStateProvider)authenticationStateProvider;
    }

    public async Task Logout()
    {
        var response = await _client.LogoutUserAsync(CancellationToken.None);

        if (!response.Succeeded)
        {
            // handle fail?
            return;
        }

        await _localStorageService.RemoveItemAsync("authToken");
        _authenticationStateProvider.NotifyUserLogout();
    }

    public async Task<bool> SignIn(SignInForm formData)
    {
        var response = await _client.LoginUserAsync(
            new AuthUserRequest()
            {
                Email = formData.Email,
                Password = formData.Password
            }, 
            CancellationToken.None);
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return false;
        }

        var token = response.Content;
        // get token
        // parse token

        // set user signed with token claims

        var claims = JwtParser.ParseClaimsFromJwt(token);
        if (claims.Count <= 0) return false;

        await _localStorageService.SetItemAsStringAsync("authToken", token);
        ClaimsIdentity identity = new(claims, "Some Auth Policy Type");
        _authenticationStateProvider.NotifyUserSignIn(identity);

        return true;
    }

    public async Task<bool> SignUp(SignUpForm formData)
    {
        var response = await _client.RegisterUserAsync(
            new AuthUserRequest()
            {
                Email = formData.Email,
                Name = formData.Username,
                Password = formData.Password
            },
            CancellationToken.None);
        if (!response.Succeeded)
        {
            // TODO: handle failure
            return false;
        }
        
        return true;
    }
}