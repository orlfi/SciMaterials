using System.Net.Http.Json;
using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using SciMaterials.UI.BWASM.Models;
using SciMaterials.Contracts.API.Constants;

namespace SciMaterials.UI.BWASM.Services;

public class JwtAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _client;
    private readonly ILocalStorageService _localStorageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private const string LogoutRoute = AuthApiRoute.AuthControllerName + AuthApiRoute.Logout;
    private const string SignInRoute = AuthApiRoute.AuthControllerName + AuthApiRoute.Login;
    private const string SignUpRoute = AuthApiRoute.AuthControllerName + AuthApiRoute.Register;

    public JwtAuthenticationService(HttpClient client, ILocalStorageService localStorageService, AuthenticationStateProvider authenticationStateProvider)
    {
        _client = client;
        _localStorageService = localStorageService;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task Logout()
    {
        var response = await _client.PostAsync(LogoutRoute, null);

        if (response.IsSuccessStatusCode)
        {
            // handle fail?
        }

        await _localStorageService.RemoveItemAsync("authToken");
        ((TestAuthenticationStateProvider)_authenticationStateProvider).NotifyUserLogout();
    }

    public async Task SignIn(SignInForm formData)
    {
        FormUrlEncodedContent content = new(new KeyValuePair<string, string>[]
        {
            new("email", formData.Email!),
            new("password", formData.Password!),
        });
        var response = await _client.PostAsync(SignInRoute, content);
        response.EnsureSuccessStatusCode();
        var someWhereToken = response.Content.ReadAsStringAsync();

        // get token
        // parse token

        // set user signed with token claims

        //await _localStorageService.SetItemAsStringAsync("authToken", JsonSerializer.Serialize(userInfo));

        //ClaimsIdentity userData = new(userClaims, "Some Auth Policy Type");
        //((TestAuthenticationStateProvider)_authenticationStateProvider).NotifyUserSignIn(userData);
    }

    public async Task<bool> SignUp(SignUpForm formData)
    {
        UserRequest request = new(formData.Email!, formData.Password!);
        var response = await _client.PostAsJsonAsync(SignUpRoute, request);
        response.EnsureSuccessStatusCode();

        // notify registration success
        // navigate to login page
        return false;
    }

    // TODO: Use contract model
    record UserRequest(string Email, string Password, string? PhoneNumber = null);
}