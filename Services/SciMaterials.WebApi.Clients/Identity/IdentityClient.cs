using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.AuthRoles;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.API.DTO.Clients;
using SciMaterials.Contracts.API.DTO.Passwords;
using SciMaterials.Contracts.API.Services.Identity;

namespace SciMaterials.WebApi.Clients.Identity;

public class IdentityClient : IIdentityClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<IdentityClient> _logger;

    public IdentityClient(HttpClient client, ILogger<IdentityClient> logger)
    {
        _client = client;
        _options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};
        _logger = logger;
    }

    /// <summary>
    /// Метод клиента для регистрации пользователя в Identity
    /// </summary>
    /// <param name="registerUser">Запрос на регистрацию</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> RegisterUserAsync(AuthUserRequest registerUser, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "RegisterUser {Email}", registerUser.Email);
        var content = JsonSerializer.Serialize(registerUser, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.Register}"),
            Content = bodyContent,
        };
        
        var registrationResult = await _client.SendAsync(request, cancellationToken);
        var result = await registrationResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для авторизации пользователя в Identity
    /// </summary>
    /// <param name="loginUser">Запрос на авторизацию</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> LoginUserAsync(AuthUserRequest loginUser, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "LoginUser {Email}", loginUser.Email);
        var content = JsonSerializer.Serialize(loginUser, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.Login}"),
            Content = bodyContent
        };
        
        var loginResult = await _client.SendAsync(request, cancellationToken);
        var result = await loginResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        //Прописываем токен для будущих запросов к api, пока реализовано так.
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",result.Content);
        return result;
    }

    /// <summary>
    /// Метод клиента для выхода пользователя из системы Identity
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> LogoutUserAsync(CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "LogoutUser");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.Logout}"),
        };
        
        var logoutResult = await _client.SendAsync(request, cancellationToken);
        var result = await logoutResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        _client.DefaultRequestHeaders.Authorization = null;
        return result;
    }

    /// <summary>
    /// Метод клиента для смены пароля в Identity
    /// </summary>
    /// <param name="passwordRequest">Запрос на смену пароля</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> ChangePasswordAsync(ChangePasswordRequest passwordRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "ChangePassword");
        var content = JsonSerializer.Serialize(passwordRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.ChangePassword}"),
            Content = bodyContent
        };
        
        var changePasswordResult = await _client.SendAsync(request, cancellationToken);
        var result = await changePasswordResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для создания роли пользователя в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на создание роли</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> CreateRoleAsync(AuthRoleRequest roleRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "CreateRole {RoleName}", roleRequest.RoleName);
        var content = JsonSerializer.Serialize(roleRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.CreateRole}"),
            Content = bodyContent
        };
        
        var createRoleResult = await _client.SendAsync(request, cancellationToken);
        var result = await createRoleResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для получения инф. о всех ролях в Identity
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "GetAllRoles");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.GetAllRoles}"),
        };
        
        var getAllRolesResult = await _client.SendAsync(request, cancellationToken);
        var result = await getAllRolesResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для получения инф. о роли по идентификатору в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на получение инф. о роли по идентификатору</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> GetRoleByIdAsync(AuthRoleRequest roleRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "GetRoleById {RoleId}", roleRequest.RoleId);
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.GetRoleById}"+ 
                                 $"{roleRequest.RoleId}"),
        };

        var getRolesByIdResult = await _client.SendAsync(request, cancellationToken);
        var result = await getRolesByIdResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для редактирования роли по идентификатору в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на редактирование роли по идентификатору</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> EditRoleByIdAsync(AuthRoleRequest roleRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "EditRoleById {RoleId}", roleRequest.RoleId);
        var content = JsonSerializer.Serialize(roleRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.EditRoleById}"),
            Content = bodyContent
        };
        
        var getRoleByIdResult = await _client.SendAsync(request, cancellationToken);
        var result = await getRoleByIdResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента на удаление роли по идентификатору в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на удаление роли по идентификатору</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> DeleteRoleByIdAsync(AuthRoleRequest roleRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "DeleteRoleById {RoleId}", roleRequest.RoleId);
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.DeleteRoleById}" + 
                                 $"{roleRequest.RoleId}"),
        };
        
        var deleteRoleResult = await _client.SendAsync(request, cancellationToken);
        var result = await deleteRoleResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для добавления роли к пользователю в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на добавление роли</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> AddRoleToUserAsync(AuthRoleRequest roleRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "AddRole {Role} ToUser {Email}", roleRequest.RoleName, roleRequest.Email);
        var content = JsonSerializer.Serialize(roleRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.AddRoleToUser}"),
            Content = bodyContent
        };
        
        var addRoleToUserResult = await _client.SendAsync(request, cancellationToken);
        var result = await addRoleToUserResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для удаления роли пользователя по email в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на удаление роли по email</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> DeleteUserRoleByEmailAsync(AuthRoleRequest roleRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "DeleteUserRoleByEmail {Email}", roleRequest.Email);
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.DeleteUserRoleByEmail}" + 
                                 $"{roleRequest.Email}/" + 
                                 $"{roleRequest.RoleName}"),
        };
        
        var deleteUserRoleResult = await _client.SendAsync(request, cancellationToken);
        var result = await deleteUserRoleResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для получения информации о всех ролях в системе в Identity
    /// </summary>
    /// <param name="roleRequest">Запрос на получение информации о всех пользователях</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> ListOfUserRolesAsync(AuthRoleRequest roleRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "ListOfUser {Email} Roles", roleRequest.Email);
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.ListOfUserRolesByEmail}" + 
                                 $"{roleRequest.Email}"),
        };
        
        var listOfUserRolesResult = await _client.SendAsync(request, cancellationToken);
        var result = await listOfUserRolesResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }
    
    /// <summary>
    /// Метод клиента для создания нового пользователя (админом) в Identity
    /// </summary>
    /// <param name="userRequest">Запрос на создание пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> CreateUserAsync(AuthUserRequest userRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "CreateUser {Email} {Name}", userRequest.Email, userRequest.Name);
        var content = JsonSerializer.Serialize(userRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.CreateUser}"),
            Content = bodyContent
        };
        
        var createUserResult = await _client.SendAsync(request, cancellationToken);
        var result = await createUserResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для получения информации о пользователе по email в Identity
    /// </summary>
    /// <param name="userRequest">Запрос на получение информации о пользователе</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> GetUserByEmailAsync(AuthUserRequest userRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "GetUserByEmail {Email}", userRequest.Email);
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.GetUserByEmail}" + 
                                 $"{userRequest.Email}"),
        };
        
        var getUserByEmailResult = await _client.SendAsync(request, cancellationToken);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для получения информации о всех пользователях в Identity
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "GetAllUsers");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.GetAllUsers}"),
        };
        
        var getAllUsersResult = await _client.SendAsync(request, cancellationToken);
        var result = await getAllUsersResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для изменения имени (ник нейма) пользователя в Identity
    /// </summary>
    /// <param name="editUserRequest">Запрос на изменение имени</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> EditUserNameByEmailAsync(EditUserRequest editUserRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "EditUserName {Name} ByEmail {Email}", 
            editUserRequest.EditUserInfo?.Name, editUserRequest.Email);
        var content = JsonSerializer.Serialize(editUserRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.EditUserByEmail}"),
            Content = bodyContent
        };
        
        var getUserByEmailResult = await _client.SendAsync(request, cancellationToken);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для удаления пользователя по email в Identity
    /// </summary>
    /// <param name="userRequest">Запрос на удаление</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> DeleteUserByEmail(AuthUserRequest userRequest, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "DeleteUserByEmail {Email}", userRequest.Email);
        var content = JsonSerializer.Serialize(userRequest, _options);
        var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.DeleteUserByEmail}" + 
                                 $"{userRequest.Email}"),
            Content = bodyContent
        };
        
        var getUserByEmailResult = await _client.SendAsync(request, cancellationToken);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }

    /// <summary>
    /// Метод клиента для удаления незарегистрированных пользователей в Identity
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>IdentityClientResponse</returns>
    public async Task<IdentityClientResponse> DeleteUsersWithOutConfirm(CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, "DeleteUsersWithOutConfirm");
        using var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{AuthApiRoute.AuthApiUri}" + 
                                 $"{AuthApiRoute.AuthControllerName}" + 
                                 $"{AuthApiRoute.DeleteUserWithoutConfirm}"),
        };
        
        var getUserByEmailResult = await _client.SendAsync(request, cancellationToken);
        var result = await getUserByEmailResult.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<IdentityClientResponse>(cancellationToken: cancellationToken);
        return result;
    }
}