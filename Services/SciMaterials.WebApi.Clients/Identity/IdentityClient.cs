using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.API.DTO.Clients;

namespace SciMaterials.WebApi.Clients.Identity;

public class IdentityClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _options;

    public IdentityClient(HttpClient client)
    {
        _client = client;
        _options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
    }

    public async Task<RegisterResponse> RegisterUserAsync(AuthUserRequest registerUser)
    {
        var content = JsonSerializer.Serialize(registerUser);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        
        var registrationResult = 
            await _client.PostAsync($"{AuthApiRoute.AuthApiUri}" +
                                    $"/{AuthApiRoute.AuthControllerName}" +
                                    $"{AuthApiRoute.Register}", bodyContent);
        var result = await registrationResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<RegisterResponse>();
        return result;
    }

    public async Task<LoginResponse> LoginUserAsync(AuthUserRequest loginUser)
    {
        var content = JsonSerializer.Serialize(loginUser);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        
        var loginResult = await _client.PostAsync($"{AuthApiRoute.AuthApiUri}" +
                                                  $"/{AuthApiRoute.AuthControllerName}" +
                                                  $"{AuthApiRoute.Login}", bodyContent);
        var result = await loginResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<LoginResponse>();
        return result;
    }

    public async Task<LogoutResponse> LogoutUserAsync()
    {
        var logoutResult = await _client.PostAsync($"{AuthApiRoute.AuthApiUri}" +
                                                   $"/{AuthApiRoute.AuthControllerName}" +
                                                   $"{AuthApiRoute.Logout}", null);
        var result = await logoutResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<LogoutResponse>();
        return result;
    }
}