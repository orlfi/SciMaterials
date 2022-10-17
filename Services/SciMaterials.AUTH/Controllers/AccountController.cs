using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciMaterials.AUTH.Services;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.AuthRoles;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.API.DTO.Clients;
using SciMaterials.Contracts.API.DTO.Passwords;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Auth.Controllers;

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
    private readonly ILogger<AccountController> _Logger;
    private readonly IAuthUtils _AuthUtils;

    public AccountController(
        UserManager<IdentityUser> UserManager,
        SignInManager<IdentityUser> SignInManager,
        RoleManager<IdentityRole> RoleManager,
        ILogger<AccountController> logger,
        IHttpContextAccessor ContextAccessor,
        IAuthUtils AuthUtils)
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

                return Ok(new RegisterResponse
                {
                    Succeeded = true,
                    Code = (int)ResultCodes.Ok,
                    Message = "Пройдите по ссылке, чтобы подтвердить ваш email",
                    ConfirmEmail = callback_url,
                });
            }

            _Logger.Log(LogLevel.Information, "Не удалось зарегистрировать пользователя {Email}",
                UserRequest.Email);
            return Ok(new RegisterResponse {Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Пользователя не удалось зарегистрировать {Ex}", ex);
            return Ok(new RegisterResponse {Succeeded = false, Code = (int)ResultCodes.ServerError});
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

                        return Ok(new LoginResponse
                        {
                            Succeeded = true,
                            Code = (int) ResultCodes.Ok,
                            Message = Response.Headers.Authorization = $"Bearer {session_token}"
                        });
                    }

                    _Logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Email}", UserRequest.Email);
                    return Ok(new LoginResponse{Succeeded = false, Code = (int)ResultCodes.NotFound});
                }
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Ex}", ex);
                return Ok(new LoginResponse{Succeeded = false, Code = (int)ResultCodes.ServerError});
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {Password}",
            UserRequest?.Email, UserRequest?.Password);
        return Ok(new LoginResponse{Succeeded = false, Code = (int)ResultCodes.ValidationError});
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
            return Ok(new LogoutResponse{Succeeded = true, Code = (int)ResultCodes.Ok});
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось выйти из системы {Ex}", ex);
            return Ok(new LogoutResponse{Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод смены пароля пользователя
    /// </summary>
    /// <param name="PasswordRequest">Запрос на смену пароля</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin, user")]
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest? PasswordRequest)
    {
        if (PasswordRequest is not null)
        {
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
                        return Ok(await Result.SuccessAsync("Пароль пользователя успешно изменен"));
                    }

                    _Logger.Log(LogLevel.Information, "Не удалось изменить пароль {CurrentPassword}, {NewPassword}",
                        PasswordRequest.CurrentPassword, PasswordRequest.NewPassword);
                    return Ok(await Result.SuccessAsync("Не удалось изменить пароль"));
                }

                _Logger.Log(LogLevel.Information,
                    "Не удалось получить информацию о пользователе {IdentityUser} или ваша почта не подтверждена {isEmailCorfirmed}",
                    identity_user, is_email_confirmed);
                return Ok(await Result.SuccessAsync("Не удалось получить информацию о пользователе или ваша почта не подтверждена"));
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при смене пароля {Ex}", ex);
                return Ok(await Result.SuccessAsync("Произошла ошибка при смене пароля"));
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные {CurrentPasswoprd}, {NewPassword}",
            PasswordRequest?.CurrentPassword, PasswordRequest?.NewPassword);
        return Ok(await Result.SuccessAsync("Некорректно введены данные"));
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
        if (UserId is not { Length: > 0 } || ConfirmToken is not { Length: > 0 })
        {
            try
            {
                var identity_user = await _UserManager.FindByIdAsync(UserId);
                if (identity_user is not null)
                {
                    var identity_result = await _UserManager.ConfirmEmailAsync(identity_user, ConfirmToken);
                    if (identity_result.Succeeded)
                    {
                        return Ok("Почта успешно подтверждена");
                    }

                    _Logger.Log(LogLevel.Information, "Не удалось подтвердить email пользователя");
                    return Ok("Не удалось подтвердить email пользователя");
                }

                _Logger.Log(LogLevel.Information, "Не удалось найти пользователя в системе {UserId}, {ConfirmTokin}",
                    UserId, ConfirmToken);
                return Ok("Не удалось найти пользователя в системе");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при подтверждении почты {Ex}", ex);
                return Ok("Произошла ошибка при подтверждении почты");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректные данные {UserId}, {ConfirmTokin}",
            UserId, ConfirmToken);
        return Ok("Некорректные данные");
    }

    /// <summary>
    /// Метод создания роли для пользователя
    /// </summary>
    /// <param name="RoleRequest">Запро на создание роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<IActionResult> CreateRoleAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is not null)
        {
            try
            {
                var identity_result = await _RoleManager.CreateAsync(new IdentityRole(RoleRequest.RoleName));
                if (identity_result.Succeeded)
                {
                    return Ok("Роль для пользователя успешно создана");
                }

                _Logger.Log(LogLevel.Information, "Не удалось создать роль");
                return Ok("Не удалось создать роль");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при создании роли {Ex}", ex);
                return Ok("Произошла ошибка при создании роли");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные {RoleName}", RoleRequest);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод получения всех ролей в системе
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpGet(AuthApiRoute.GetAllRoles)]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        try
        {
            var identity_roles_list = await _RoleManager.Roles.ToListAsync();
            if (identity_roles_list.Count != 0)
            {
                return Ok(identity_roles_list);
            }

            _Logger.Log(LogLevel.Information, "Не удалось получить список ролей");
            return Ok("Не удалось получить список ролей");
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при запросе ролей {Ex}", ex);
            return Ok("Произошла ошибка при запросе ролей");
        }
    }

    /// <summary>
    /// Метод получения роли по идентификатору
    /// </summary>
    /// <param name="RoleRequest">Запрос на получение информации о роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpGet(AuthApiRoute.GetRoleById)]
    public async Task<IActionResult> GetRoleByIdAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is not null)
        {
            try
            {
                var identity_role = await _RoleManager.FindByIdAsync(RoleRequest.RoleId);
                if (identity_role is not null)
                {
                    return Ok(identity_role);
                }

                _Logger.Log(LogLevel.Information, "Не удалось получить роль");
                return Ok("Не удалось получить роль");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при запросе роли {Ex}", ex);
                return Ok("Произошла ошибка при запросе роли");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные {RoleId}", RoleRequest?.RoleId);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод редактирования роли по идентификатору
    /// </summary>
    /// <param name="RoleRequest">Запрос на редактирование роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.EditRoleById)]
    public async Task<IActionResult> EditRoleByIdAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is not null)
        {
            try
            {
                var found_role = await _RoleManager.FindByIdAsync(RoleRequest.RoleId);
                found_role.Name = RoleRequest.RoleName;

                var identity_result = await _RoleManager.UpdateAsync(found_role);
                if (identity_result.Succeeded)
                {
                    return Ok($"Роль успешно изменена с {found_role} на {RoleRequest.RoleName}");
                }

                _Logger.Log(LogLevel.Information, "Не удалось изменить роль");
                return Ok("Не удалось изменить роль");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при редактировании роли {Ex}", ex);
                return Ok("Произошла ошибка при редактировании роли");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные {RoleId}, {RoleName}",
            RoleRequest?.RoleId, RoleRequest.RoleName);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод удаления роли по идентификатору
    /// </summary>
    /// <param name="RoleRequest">Запрос на удаление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteRoleById)]
    public async Task<IActionResult> DeleteRoleByIdAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is not null)
        {
            try
            {
                var identity_role = await _RoleManager.FindByIdAsync(RoleRequest.RoleId);
                if (identity_role is not null)
                {
                    var identity_result = await _RoleManager.DeleteAsync(identity_role);
                    if (identity_result.Succeeded)
                    {
                        return Ok("Роль успешно удалена");
                    }

                    _Logger.Log(LogLevel.Information, "Не удалось удалить роль");
                    return Ok("Не удалось удалить роль");
                }

                _Logger.Log(LogLevel.Information, "Не удалось найти роль");
                return Ok("Не удалось найти роль");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении роли {Ex}", ex);
                return Ok("Произошла ошибка при удалении роли");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные");
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод добавления роли к пользователю
    /// </summary>
    /// <param name="RoleRequest">Запрос на добавление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<IActionResult> AddRoleToUserAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is not null)
        {
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
                                return Ok(role_added_result);
                            }
                        }
                    }
                }

                _Logger.Log(LogLevel.Information, "Некорректно введены данные");
                return Ok("Некорректно введены данные");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при добавлении роли к пользователю {Ex}", ex);
                return Ok("Произошла ошибка при добавлении роли пользователю");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {RoleName}",
            RoleRequest?.Email, RoleRequest?.RoleName);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод удаления роли у пользователя
    /// </summary>
    /// <param name="RoleRequest">Запрос на удаление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteUserRole)]
    public async Task<IActionResult> DeleteUserRoleAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is not null)
        {
            try
            {
                var identity_user = await _UserManager.FindByEmailAsync(RoleRequest.Email);
                var user_roles_list = await _UserManager.GetRolesAsync(identity_user);
                var system_roles_list = await _RoleManager.Roles.ToListAsync();
                if (user_roles_list.Contains(RoleRequest.RoleName))
                {
                    var is_role_contains_in_system = system_roles_list.Select(x =>
                        x.Name.Contains(RoleRequest.RoleName!));
                    foreach (var is_role in is_role_contains_in_system)
                    {
                        if (is_role)
                        {
                            var role_removed_result = await _UserManager.RemoveFromRoleAsync(identity_user, RoleRequest.RoleName);
                            if (role_removed_result.Succeeded)
                            {
                                return Ok(role_removed_result);
                            }
                        }
                    }
                }

                _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {RoleName}",
                    RoleRequest.Email, RoleRequest.RoleName);
                return Ok("Некорректно введены данные");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении роли пользователю {Ex}", ex);
                return Ok("Произошла ошибка при удалении роли пользователю");
            }
        }

        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод получения всех ролей пользователя
    /// </summary>
    /// <param name="RoleRequest">Запрос на получение списка ролей</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpGet(AuthApiRoute.ListOfUserRoles)]
    public async Task<IActionResult> ListOfUserRolesAsync([FromBody] AuthRoleRequest? RoleRequest)
    {
        if (RoleRequest is not null)
        {
            try
            {
                var identity_user = await _UserManager.FindByEmailAsync(RoleRequest.Email);
                if (identity_user is not null)
                {
                    var user_roles_list = await _UserManager.GetRolesAsync(identity_user);
                    if (user_roles_list.Count != 0)
                    {
                        return Ok(user_roles_list.ToList());
                    }

                    _Logger.Log(LogLevel.Information, "Не удалось получить список ролей");
                    return Ok("Не удалось получить список ролей");
                }

                _Logger.Log(LogLevel.Information,
                    "Данного пользователя {IdentityUser} нет в системе, либо некорректно введены данные пользователя " +
                    "{Email}", identity_user, RoleRequest.Email);
                return Ok("Данного пользователя нет в системе, либо некорректно введены данные пользователя");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при получении списка ролей пользователей {Ex}", ex);
                return Ok("Произошла ошибка при получении списка ролей пользователей");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}", RoleRequest?.Email);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод создания пользователя админом
    /// </summary>
    /// <param name="UserRequest">Запрос админа</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.CreateUser)]
    public async Task<IActionResult> CreateUserAsync([FromBody] AuthUserRequest? UserRequest)
    {
        if (UserRequest is not null)
        {
            var action_result = await RegisterAsync(UserRequest);
            return Ok(action_result);
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
        return Ok("Некорректно введены данные пользователя");
    }

    /// <summary>
    /// Метод получения информации о пользователе
    /// </summary>
    /// <param name="UserRequest">Запрос на получение информации о пользователе</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.GetUserByEmail)]
    public async Task<IActionResult> GetUserByEmailAsync([FromBody] AuthUserRequest? UserRequest)
    {
        if (UserRequest is not null)
        {
            try
            {
                var identity_user = await _UserManager.FindByEmailAsync(UserRequest.Email);
                if (identity_user is not null)
                {
                    return Ok(identity_user);
                }

                _Logger.Log(LogLevel.Information, "Не удалось получить информации о пользователе");
                return Ok("Не удалось получить информации о пользователе");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователей {Ex}", ex);
                return Ok("Произошла ошибка при получении пользователей");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные {Email}", UserRequest?.Email);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод получения всех пользователей в системе админом
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.GetAllUsers)]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        try
        {
            var list_of_all_users = await _UserManager.Users.ToListAsync();
            return Ok(list_of_all_users);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователя {Ex}", ex);
            return Ok("Произошла ошибка при получении пользователя");
        }
    }

    /// <summary>
    /// Метод редактирования информации о пользователе
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <param name="EditUserRequest">Запрос админа</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.EditUserByEmail)]
    public async Task<IActionResult> EditUserByEmailAsync([FromBody] EditUserRequest? EditUserRequest)
    {
        if (EditUserRequest is not null)
        {
            try
            {
                var identity_user = await _UserManager.FindByEmailAsync(EditUserRequest.Email);
                if (identity_user is not null)
                {
                    identity_user.Email = EditUserRequest.EditUserInfo?.Email;
                    identity_user.UserName = EditUserRequest.EditUserInfo?.Email;

                    var identity_result = await _UserManager.UpdateAsync(identity_user);
                    if (identity_result.Succeeded)
                    {
                        return Ok("Информаци о пользователе успешно изменена");
                    }

                    _Logger.Log(LogLevel.Information, "Не удалось обновить информацию пользователя");
                    return Ok("Не удалось обновить информацию пользователя");
                }

                _Logger.Log(LogLevel.Information,
                    "Не удалось найти пользователя {Email} или некорректно введены данные", EditUserRequest.Email);
                return Ok("Не удалось найти данного пользователя или некорректно введены данные пользователя");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при обновлении информации о пользователе {Ex}", ex);
                return Ok("Произошла ошибка при обновлении информации о пользователе");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
        return Ok("Некорректно введены данные пользователя");
    }

    /// <summary>
    /// Метод удаления пользователя
    /// </summary>
    /// <param name="UserRequest">Запрос на удаление пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteUserByEmail)]
    public async Task<IActionResult> DeleteUserByEmail([FromBody] AuthUserRequest? UserRequest)
    {
        if (UserRequest is not null)
        {
            try
            {
                var identity_user = await _UserManager.FindByEmailAsync(UserRequest.Email);
                if (identity_user is not null)
                {
                    var identity_result = await _UserManager.DeleteAsync(identity_user);
                    if (identity_result.Succeeded)
                    {
                        return Ok("Пользователь успешно удален");
                    }

                    _Logger.Log(LogLevel.Information, "Не удалось удалить пользователя {Email}", UserRequest.Email);
                    return Ok("Не удалось удалить пользователя");
                }

                _Logger.Log(LogLevel.Information, "Не удалось получить информацию о пользователе {Email}", UserRequest.Email);
                return Ok("Не удалось получить информацию о пользователе");
            }
            catch (Exception ex)
            {
                _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении пользователя {Ex}", ex);
                return Ok("Произошла ошибка при удалении пользователя");
            }
        }

        _Logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
        return Ok("Некорректно введены данные пользователя");
    }

    /// <summary>
    /// Метод удаления пользователей без регистрации (для очистки БД)
    /// </summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteUserWithoutConfirmation)]
    public async Task<IActionResult> DeleteUsersWithOutConfirmation()
    {
        try
        {
            var users_to_delete_list = await _UserManager
                .Users.Where(x =>
                    x.EmailConfirmed.Equals(false))
                .ToListAsync();
            foreach (var user in users_to_delete_list)
            {
                if (user.EmailConfirmed is false)
                {
                    await _UserManager.DeleteAsync(user);
                }
            }

            return Ok("Пользователи без регистрации успешно удалены");
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось удалить пользователей из-за ошибки {Ex}", ex);
            return Ok("Не удалось удалить пользователей из-за ошибки");
        }
    }
}