using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.AuthRoles;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.API.DTO.Clients;
using SciMaterials.Contracts.API.DTO.Passwords;
using SciMaterials.Contracts.Enums;
using SciMaterials.UI.MVC.Identity.Services;

namespace SciMaterials.UI.MVC.Identity.Controllers;

/// <summary>
/// Контроллер для регистрации и авторизации в системе аутентификации
/// </summary>
[ApiController]
[Route(AuthApiRoute.AuthControllerName)]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly SignInManager<IdentityUser> _SignInManager;
    private readonly RoleManager<IdentityRole> _RoleManager;
    private readonly IHttpContextAccessor _ContextAccessor;
    private readonly IAuthUtils _AuthUtils;
    private readonly ILogger<AccountController> _Logger;

    public AccountController(
        UserManager<IdentityUser> UserManager,
        SignInManager<IdentityUser> SignInManager,
        RoleManager<IdentityRole> RoleManager,
        IHttpContextAccessor ContextAccessor,
        IAuthUtils AuthUtils,
        ILogger<AccountController> logger)
    {
        _UserManager = UserManager;
        _SignInManager = SignInManager;
        _RoleManager = RoleManager;
        _Logger = logger;
        _ContextAccessor = ContextAccessor;
        _AuthUtils = AuthUtils;
    }

    /// <summary>
    /// Метод регистрации пользователя
    /// </summary>
    /// <param name="UserRequest">Запрос пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Register)]
    public async Task<IActionResult> RegisterAsync([FromBody] AuthUserRequest? UserRequest)
    {
        if (UserRequest is null)
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {Password}",
                UserRequest?.Email, UserRequest?.Password);
            return Ok(new {Message = $"{ResultCodes.ValidationError}"});
        }
        
        try
        {
            var identity_user = new IdentityUser{Email = UserRequest.Email, UserName = UserRequest.Email};

            var identity_result = await _UserManager.CreateAsync(identity_user, UserRequest.Password);
            if (identity_result.Succeeded)
            {
                await _UserManager.AddToRoleAsync(identity_user, AuthApiRoles.User);
                await _SignInManager.SignInAsync(identity_user, false);

                var email_confirm_token = await _UserManager.GenerateEmailConfirmationTokenAsync(identity_user);

                var callback_url = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = identity_user.Id, confirmToken = email_confirm_token }, protocol:
                    HttpContext.Request.Scheme);

                return Ok(new IdentityClientResponse
                {
                    Succeeded = true,
                    Code = (int)ResultCodes.Ok,
                    Content = "Пройдите по ссылке, чтобы подтвердить ваш email",
                    ConfirmEmail = callback_url,
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось зарегистрировать пользователя {Email}",
                UserRequest.Email);
            return Ok(new IdentityClientResponse {Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Пользователя не удалось зарегистрировать {Ex}", ex);
            return Ok(new IdentityClientResponse {Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод авторизации пользователя
    /// </summary>
    /// <param name="UserRequest">Запрос пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Login)]
    public async Task<IActionResult> LoginAsync([FromBody] AuthUserRequest? UserRequest)
    {
        if (UserRequest is not null)
        {
            try
            {
                var identity_user = await _UserManager.FindByEmailAsync(UserRequest.Email);
                if (identity_user is not null)
                {
                    var identity_roles = await _UserManager.GetRolesAsync(identity_user);

                    var sign_in_result = await _SignInManager.PasswordSignInAsync(
                        userName: UserRequest.Email,
                        password: UserRequest.Password,
                        isPersistent: true,
                        lockoutOnFailure: false);

                    if (sign_in_result.Succeeded)
                    {
                        var session_token = _AuthUtils.CreateSessionToken(identity_user, identity_roles);
                        
                        return Ok(new IdentityClientResponse
                        {
                            Succeeded = true,
                            Code = (int) ResultCodes.Ok,
                            Content = session_token
                        });
                    }

                    _Logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Email}", UserRequest.Email);
                    return Ok(new IdentityClientResponse{Succeeded = false, Code = (int)ResultCodes.NotFound});
                }
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Ex}", ex);
                return Ok(new IdentityClientResponse{Succeeded = false, Code = (int)ResultCodes.ServerError});
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {Password}",
            UserRequest?.Email, UserRequest?.Password);
        return Ok(new IdentityClientResponse{Succeeded = false, Code = (int)ResultCodes.ValidationError});
    }

    /// <summary>
    /// Метод выхода пользователя из системы
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Logout)]
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            await _SignInManager.SignOutAsync();
            return Ok(new IdentityClientResponse{Succeeded = true, Code = (int)ResultCodes.Ok});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось выйти из системы {Ex}", ex);
            return Ok(new IdentityClientResponse{Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод смены пароля пользователя
    /// </summary>
    /// <param name="PasswordRequest">Запрос на смену пароля</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest? PasswordRequest)
    {
        if (PasswordRequest is null)
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные {CurrentPasswoprd}, {NewPassword}",
                PasswordRequest?.CurrentPassword, PasswordRequest?.NewPassword);
            return Ok(new IdentityClientResponse() {Succeeded = false, Code = (int) ResultCodes.ValidationError});
        }

        try
        {
            var current_user_name = _ContextAccessor.HttpContext?.User.Identity?.Name;

            var identity_user = await _UserManager.FindByNameAsync(current_user_name);
            var is_email_confirmed = await _UserManager.IsEmailConfirmedAsync(identity_user);
            if (current_user_name is not { Length: > 0 } ||
                identity_user is not null ||
                is_email_confirmed)
            {
                var identity_result = await _UserManager.ChangePasswordAsync(
                    identity_user!,
                    PasswordRequest.CurrentPassword,
                    PasswordRequest.NewPassword);
                if (identity_result.Succeeded)
                {
                    return Ok(new IdentityClientResponse()
                    {
                        Succeeded = true,
                        Code = (int)ResultCodes.Ok,
                        Content = "Пароль успешно изменен",
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось изменить пароль {CurrentPassword}, {NewPassword}",
                    PasswordRequest.CurrentPassword, PasswordRequest.NewPassword);
                return Ok(new IdentityClientResponse() {Succeeded = false, Code = (int) ResultCodes.ValidationError});
            }

            _Logger.Log(LogLevel.Information,
                "Не удалось получить информацию о пользователе {IdentityUser} или ваша почта не подтверждена {isEmailCorfirmed}",
                identity_user, is_email_confirmed);
            return Ok(new IdentityClientResponse() {Succeeded = false, Code = (int) ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при смене пароля {Ex}", ex);
            return Ok(new IdentityClientResponse() {Succeeded = false, Code = (int) ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод подтверждения почты пользователя, когда пользователь проходит по сформированной ссылке
    /// </summary>
    /// <param name="UserId">Идентификатор пользователя в системе</param>
    /// <param name="ConfirmToken">Токен подтверждения</param>
    /// <returns>Status 200 OK.</returns>
    [HttpGet(AuthApiRoute.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmailAsync(string UserId, string ConfirmToken)
    {
        if (UserId is not {Length: > 0} || ConfirmToken is not {Length: > 0})
        {
            _Logger.Log(LogLevel.Information, "Некорректные данные {UserId}, {ConfirmTokin}",
                UserId, ConfirmToken);
            return Ok(new IdentityClientResponse() {Succeeded = false, Code = (int) ResultCodes.ValidationError});
        }

        try
        {
            var identity_user = await _UserManager.FindByIdAsync(UserId);
            if (identity_user is not null)
            {
                var identity_result = await _UserManager.ConfirmEmailAsync(identity_user, ConfirmToken);
                if (identity_result.Succeeded)
                {
                    return Ok(new IdentityClientResponse()
                    {
                        Succeeded = true, Code = (int) ResultCodes.Ok, Content = "Учетная запись успешно подтверждена"
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось подтвердить email пользователя");
                return Ok(new IdentityClientResponse() {Succeeded = false, Code = (int) ResultCodes.NotFound});
            }

            _Logger.Log(LogLevel.Information, "Не удалось найти пользователя в системе {UserId}, {ConfirmTokin}",
                UserId, ConfirmToken);
            return Ok(new IdentityClientResponse() {Succeeded = false, Code = (int) ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при подтверждении почты {Ex}", ex);
            return Ok(new IdentityClientResponse() {Succeeded = false, Code = (int) ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод создания роли для пользователя
    /// </summary>
    /// <param name="RoleRequest">Запро на создание роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<IActionResult> CreateRoleAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is null)
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные {RoleName}", RoleRequest);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }

        try
        {
            var identity_result = await _RoleManager.CreateAsync(new IdentityRole(RoleRequest.RoleName?.ToLower()));
            if (identity_result.Succeeded)
            {
                return Ok(new IdentityClientResponse()
                {
                    Succeeded = true, Code = (int)ResultCodes.Ok, Content = "Роль для пользователя успешно создана"
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось создать роль");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ApiError});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при создании роли {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод получения всех ролей в системе
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet(AuthApiRoute.GetAllRoles)]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        try
        {
            var identity_roles_list = await _RoleManager.Roles.ToListAsync();
            if (identity_roles_list.Count != 0)
            {
                var jsonOpt = new JsonSerializerOptions() { WriteIndented = true};
                var content = JsonSerializer.Serialize(identity_roles_list, jsonOpt);
                return Ok(new IdentityClientResponse(){Succeeded = true, Code = (int)ResultCodes.Ok, Content = content});
            }

            _Logger.Log(LogLevel.Information, "Не удалось получить список ролей");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при запросе ролей {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод получения роли по идентификатору
    /// </summary>
    /// <param name="roleId">Идентификатор роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetRoleById}"+"{roleId}")]
    public async Task<IActionResult> GetRoleByIdAsync(string roleId)
    {
        if (string.IsNullOrEmpty(roleId))
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные {RoleId}", roleId);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ValidationError});
        }

        try
        {
            var identity_role = await _RoleManager.FindByIdAsync(roleId);
            if (identity_role is not null)
            {
                var jsonOpt = new JsonSerializerOptions() { WriteIndented = true};
                var content = JsonSerializer.Serialize(identity_role, jsonOpt);
                return Ok(new IdentityClientResponse(){Succeeded = true, Code = (int)ResultCodes.Ok, Content = content});
            }

            _Logger.Log(LogLevel.Information, "Не удалось получить роль");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при запросе роли {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод редактирования роли по идентификатору
    /// </summary>
    /// <param name="RoleRequest">Запрос на редактирование роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPut(AuthApiRoute.EditRoleById)]
    public async Task<IActionResult> EditRoleByIdAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is null)
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные {RoleId}, {RoleName}",
                RoleRequest?.RoleId, RoleRequest.RoleName);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ValidationError});
        }

        try
        {
            var found_role = await _RoleManager.FindByIdAsync(RoleRequest.RoleId);
            found_role.Name = RoleRequest.RoleName;

            var identity_result = await _RoleManager.UpdateAsync(found_role);
            if (identity_result.Succeeded)
            {
                return Ok(new IdentityClientResponse()
                {
                    Succeeded = true, 
                    Code = (int)ResultCodes.Ok, 
                    Content = $"Роль успешно изменена"
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось изменить роль");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при редактировании роли {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод удаления роли по идентификатору
    /// </summary>
    /// <param name="roleId">Запрос на удаление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteRoleById}"+"{roleId}")]
    public async Task<IActionResult> DeleteRoleByIdAsync(string roleId)
    {
        if (string.IsNullOrEmpty(roleId))
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ValidationError});
        }

        try
        {
            var identity_role = await _RoleManager.FindByIdAsync(roleId);
            if (identity_role is not null)
            {
                var identity_result = await _RoleManager.DeleteAsync(identity_role);
                if (identity_result.Succeeded)
                {
                    return Ok(new IdentityClientResponse()
                    {
                        Succeeded = true, Code = (int)ResultCodes.Ok, Content = "Роль успешно удалена"
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось удалить роль");
                return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ApiError});
            }

            _Logger.Log(LogLevel.Information, "Не удалось найти роль");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении роли {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод добавления роли к пользователю
    /// </summary>
    /// <param name="RoleRequest">Запрос на добавление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<IActionResult> AddRoleToUserAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is null)
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {RoleName}",
                RoleRequest?.Email, RoleRequest?.RoleName);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ValidationError});
        }

        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(RoleRequest.Email);
            var user_roles_list = await _UserManager.GetRolesAsync(identity_user);
            var system_roles_list = await _RoleManager.Roles.ToListAsync();
            if (!user_roles_list.Contains(RoleRequest.RoleName))
            {
                var is_role_contains_in_system = system_roles_list.Select(x =>
                    x.Name.Contains(RoleRequest.RoleName!.ToLower()));
                foreach (var is_role in is_role_contains_in_system)
                {
                    if (is_role)
                    {
                        var role_added_result = await _UserManager.AddToRoleAsync(identity_user, RoleRequest.RoleName!.ToLower());
                        if (role_added_result.Succeeded)
                        {
                            return Ok(new IdentityClientResponse()
                            {
                                Succeeded = true, Code = (int)ResultCodes.Ok, Content = "Роль успешно добавлена"
                            });
                        }
                    }
                }
            }

            _Logger.Log(LogLevel.Information, "Некорректно введены данные");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при добавлении роли к пользователю {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод удаления роли у пользователя
    /// </summary>
    /// <param name="email">Почта</param>
    /// <param name="roleName">Название роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteUserRoleByEmail}"+"{email}/{roleName}")]
    public async Task<IActionResult> DeleteUserRoleByEmailAsync(string email, string roleName)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(roleName))
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}", email);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ValidationError});
        }

        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(email);
            var user_roles_list = await _UserManager.GetRolesAsync(identity_user);
            var system_roles_list = await _RoleManager.Roles.ToListAsync();
            if (user_roles_list.Contains(roleName.ToLower()))
            {
                var is_role_contains_in_system = system_roles_list.Select(x =>
                    x.Name.Contains(roleName.ToLower()));
                foreach (var is_role in is_role_contains_in_system)
                {
                    if (is_role)
                    {
                        var role_removed_result = await _UserManager.RemoveFromRoleAsync(identity_user, roleName);
                        if (role_removed_result.Succeeded)
                        {
                            return Ok(new IdentityClientResponse()
                            {
                                Succeeded = true, Code = (int)ResultCodes.Ok, Content = "Роль успешно удалена"
                            });
                        }
                    }
                }
            }

            _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {RoleName}",
                email, roleName);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ValidationError});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении роли пользователю {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод получения всех ролей пользователя
    /// </summary>
    /// <param name="email">Почта</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.ListOfUserRolesByEmail}"+"{email}")]
    public async Task<IActionResult> ListOfUserRolesByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}", email);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ValidationError});
        }

        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(email);
            if (identity_user is not null)
            {
                var user_roles_list = await _UserManager.GetRolesAsync(identity_user);
                if (user_roles_list.Count != 0)
                {
                    var jsonOpt = new JsonSerializerOptions() { WriteIndented = true};
                    var content = JsonSerializer.Serialize(user_roles_list, jsonOpt);
                    return Ok(new IdentityClientResponse(){Succeeded = true, Code = (int)ResultCodes.Ok, Content = content});
                }

                _Logger.Log(LogLevel.Information, "Не удалось получить список ролей");
                return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
            }

            _Logger.Log(LogLevel.Information,
                "Данного пользователя {IdentityUser} нет в системе, либо некорректно введены данные пользователя " +
                "{Email}", identity_user, email);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при получении списка ролей пользователей {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод создания пользователя админом
    /// </summary>
    /// <param name="UserRequest">Запрос админа</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.CreateUser)]
    public async Task<IActionResult> CreateUserAsync([FromBody] AuthUserRequest? UserRequest)
    {
        if (UserRequest is null)
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ValidationError});
        }
        
        var action_result = await RegisterAsync(UserRequest);
        if (action_result is not null)
        {
            var result = action_result as OkObjectResult;
            var response = result?.Value as IdentityClientResponse;
            return Ok(response);
        }
        
        _Logger.Log(LogLevel.Information, "Не удалось создать пользователя");
        return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
    }

    /// <summary>
    /// Метод получения информации о пользователе
    /// </summary>
    /// <param name="email">Почта</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetUserByEmail}"+"{email}")]
    public async Task<IActionResult> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}", email);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ValidationError});
        }

        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(email);
            if (identity_user is not null)
            {
                var jsonOpt = new JsonSerializerOptions() { WriteIndented = true};
                var content = JsonSerializer.Serialize(identity_user, jsonOpt);
                return Ok(new IdentityClientResponse(){Succeeded = true, Code = (int)ResultCodes.Ok, Content = content});
            }

            _Logger.Log(LogLevel.Information, "Не удалось получить информации о пользователе");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователей {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод получения всех пользователей в системе админом
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet(AuthApiRoute.GetAllUsers)]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        try
        {
            var list_of_all_users = await _UserManager.Users.ToListAsync();
            var jsonOpt = new JsonSerializerOptions() { WriteIndented = true};
            var content = JsonSerializer.Serialize(list_of_all_users, jsonOpt);
            return Ok(new IdentityClientResponse(){Succeeded = true, Code = (int)ResultCodes.Ok, Content = content});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователя {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод редактирования информации о пользователе
    /// </summary>
    /// <param name="EditUserRequest">Запрос админа</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPut(AuthApiRoute.EditUserByEmail)]
    public async Task<IActionResult> EditUserNameByEmailAsync([FromBody] EditUserRequest? EditUserRequest)
    {
        if (EditUserRequest is null)
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }

        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(EditUserRequest.Email);
            if (identity_user is not null)
            {
                identity_user.Email = EditUserRequest.Email;
                identity_user.UserName = EditUserRequest.EditUserInfo?.Name;

                var identity_result = await _UserManager.UpdateAsync(identity_user);
                if (identity_result.Succeeded)
                {
                    return Ok(new IdentityClientResponse()
                    {
                        Succeeded = true, Code = (int) ResultCodes.Ok, Content = "Информаци о пользователе успешно изменена"
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось обновить информацию пользователя");
                return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
            }

            _Logger.Log(LogLevel.Information,
                "Не удалось найти пользователя {Email} или некорректно введены данные", EditUserRequest.Email);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при обновлении информации о пользователе {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод удаления пользователя
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteUserByEmail}"+"{email}")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            _Logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }

        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(email);
            if (identity_user is not null)
            {
                var identity_result = await _UserManager.DeleteAsync(identity_user);
                if (identity_result.Succeeded)
                {
                    return Ok(new IdentityClientResponse()
                    {
                        Succeeded = true, Code = (int)ResultCodes.Ok, Content = "Пользователь успешно удален"
                    });
                }

                _Logger.Log(LogLevel.Information, "Не удалось удалить пользователя {Email}", email);
                return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
            }

            _Logger.Log(LogLevel.Information, "Не удалось получить информацию о пользователе {Email}", email);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении пользователя {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод удаления пользователей без регистрации (для очистки БД)
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete(AuthApiRoute.DeleteUserWithoutConfirm)]
    public async Task<IActionResult> DeleteUsersWithOutConfirmation()
    {
        try
        {
            var users_to_delete_list = await _UserManager
                .Users.Where(x => x.EmailConfirmed.Equals(false))
                .ToListAsync();
            foreach (var user in users_to_delete_list)
            {
                if (user.EmailConfirmed is false)
                {
                    await _UserManager.DeleteAsync(user);
                }
            }

            return Ok(new IdentityClientResponse()
            {
                Succeeded = true, Code = (int)ResultCodes.Ok, Content = "Пользователи без регистрации успешно удалены"
            });
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось удалить пользователей из-за ошибки {Ex}", ex);
            return Ok(new IdentityClientResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }
}