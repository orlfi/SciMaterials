using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciMaterials.Contracts;
using SciMaterials.DAL.AUTH.Contracts;
using SciMaterials.Contracts.Identity.API;
using SciMaterials.Contracts.Identity.API.Requests.Roles;
using SciMaterials.Contracts.Identity.API.Requests.Users;
using SciMaterials.Contracts.Identity.API.Responses.DTO;
using SciMaterials.Contracts.Identity.API.Responses.User;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.MVC.Identity.Controllers;

/// <summary>Контроллер для регистрации и авторизации в системе аутентификации</summary>
[ApiController]
[Route(AuthApiRoute.AuthControllerName)]
public class AccountController : Controller, IIdentityApi
{
    private readonly UserManager<IdentityUser> _UserManager;
    private readonly SignInManager<IdentityUser> _SignInManager;
    private readonly RoleManager<IdentityRole> _RoleManager;
    private readonly IHttpContextAccessor _ContextAccessor;
    private readonly IAuthUtils _authUtilits;
    private readonly ILogger<AccountController> _Logger;

    public AccountController(
        UserManager<IdentityUser> UserManager,
        SignInManager<IdentityUser> SignInManager,
        RoleManager<IdentityRole> RoleManager,
        IHttpContextAccessor ContextAccessor,
        IAuthUtils authUtilits,
        ILogger<AccountController> Logger)
    {
        _UserManager = UserManager;
        _SignInManager = SignInManager;
        _RoleManager = RoleManager;
        _Logger = Logger;
        _ContextAccessor = ContextAccessor;
        _authUtilits = authUtilits;
    }

    /// <summary>Метод регистрации пользователя</summary>
    /// <param name="RegisterRequest">Запрос пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Register)]
    public async Task<Result<RegisterUserResponse>> RegisterUserAsync([FromBody] RegisterRequest RegisterRequest, CancellationToken Cancel = default)
    {
        try
        {
            var identity_user = new IdentityUser { Email = RegisterRequest.Email, UserName = RegisterRequest.NickName };
            var identity_result = await _UserManager.CreateAsync(identity_user, RegisterRequest.Password);
            if (identity_result.Succeeded)
            {
                await _UserManager.AddToRoleAsync(identity_user, AuthApiRoles.User);
                var email_confirm_token = await _UserManager.GenerateEmailConfirmationTokenAsync(identity_user);

                var callback_url = Url.Action(
                    action: "ConfirmEmail",
                    controller: "Accounts",
                    values: new { UserId = identity_user.Id, ConfirmToken = email_confirm_token },
                    protocol: HttpContext.Request.Scheme);

                return Result<RegisterUserResponse>.Success(new RegisterUserResponse(callback_url));
            }

            var errors = identity_result.Errors.Select(e => e.Description).ToArray();
            _Logger.Log(LogLevel.Information, "Не удалось зарегистрировать пользователя {Email}: {errors}",
                RegisterRequest.Email,
                string.Join(",", errors));

            return Result<RegisterUserResponse>.Failure(Errors.Identity.Register.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Пользователя не удалось зарегистрировать {Ex}", ex);
            return Result<RegisterUserResponse>.Failure(Errors.Identity.Register.Unhandled);
        }
    }

    /// <summary>Метод авторизации пользователя</summary>
    /// <param name="LoginRequest">Запрос пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Login)]
    public async Task<Result<LoginUserResponse>> LoginUserAsync([FromBody] LoginRequest? LoginRequest, CancellationToken Cancel = default)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(LoginRequest.Email);
            if (identity_user is null)
            {
                _Logger.Log(
                    LogLevel.Information,
                    "Некорректно введены данные {Email}, {Password}",
                    LoginRequest.Email,
                    LoginRequest.Password);
                return Result<LoginUserResponse>.Failure(Errors.Identity.Login.UserNotFound);
            }

            if (!await CheckIsEmailConfirmedAsync(identity_user))
            {
                _Logger.Log(
                    LogLevel.Information,
                    "Email не подтверждён {Email}",
                    LoginRequest.Email);
                return Result<LoginUserResponse>.Failure(Errors.Identity.Login.EmailNotConfirmed);
            }

            var sign_in_result = await _SignInManager.PasswordSignInAsync(
            userName: identity_user.UserName,
            password: LoginRequest.Password,
            isPersistent: true,
            lockoutOnFailure: false);

            if (sign_in_result.Succeeded)
            {
                var identity_roles = await _UserManager.GetRolesAsync(identity_user);
                var session_token = _authUtilits.CreateSessionToken(identity_user, identity_roles);
                return Result<LoginUserResponse>.Success(new LoginUserResponse(session_token));
            }

            _Logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Email}", LoginRequest.Email);
            return Result<LoginUserResponse>.Failure(Errors.Identity.Login.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Ex}", ex);
            return Result<LoginUserResponse>.Failure(Errors.Identity.Login.Unhandled);
        }
    }

    /// <summary>Метод выхода пользователя из системы</summary>
    /// <returns>Status 200 OK.</returns>
    [HttpPost(AuthApiRoute.Logout)]
    public async Task<Result> LogoutUserAsync(CancellationToken Cancel = default)
    {
        try
        {
            await _SignInManager.SignOutAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось выйти из системы {Ex}", ex);
            return Result.Failure(Errors.Identity.Logout.Unhandled);
        }
    }

    /// <summary>Метод смены пароля пользователя</summary>
    /// <param name="ChangePasswordRequest">Запрос на смену пароля</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<Result> ChangePasswordAsync([FromBody] ChangePasswordRequest ChangePasswordRequest, CancellationToken Cancel = default)
    {
        try
        {
            var current_user_name = _ContextAccessor.HttpContext?.User.Identity?.Name;
            if (current_user_name is not { Length: > 0 })
            {
                _Logger.Log(
                    LogLevel.Warning,
                    "Change password request called without authorization data");
                return Result.Failure(Errors.Identity.ChangePassword.MissAuthorizationData);
            }

            var identity_user = await _UserManager.FindByNameAsync(current_user_name);
            if (identity_user is null)
            {
                _Logger.Log(
                    LogLevel.Information,
                    "Некорректно введены данные {Email}",
                    current_user_name);
                return Result.Failure(Errors.Identity.ChangePassword.NotFound);
            }

            if (!await CheckIsEmailConfirmedAsync(identity_user))
            {
                _Logger.Log(
                    LogLevel.Information,
                    "Email не подтверждён {Email}",
                    identity_user.Email);
                return Result.Failure(Errors.Identity.ChangePassword.EmailNotConfirmed);
            }

            var identity_result = await _UserManager.ChangePasswordAsync(
                identity_user,
                ChangePasswordRequest.CurrentPassword,
                ChangePasswordRequest.NewPassword);

            if (identity_result.Succeeded)
            {
                await _SignInManager.RefreshSignInAsync(identity_user);

                return Result.Success();
            }

            _Logger.Log(
                LogLevel.Information,
                "Не удалось изменить пароль {CurrentPassword}, {NewPassword}",
                ChangePasswordRequest.CurrentPassword,
                ChangePasswordRequest.NewPassword);
            return Result.Failure(Errors.Identity.ChangePassword.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при смене пароля {Ex}", ex);
            return Result.Failure(Errors.Identity.ChangePassword.Unhandled);
        }
    }

    /// <summary>Метод обновления токена пользователя</summary>
    /// <returns>Status 200 OK.</returns>
    [HttpGet(AuthApiRoute.RefreshToken)]
    public async Task<Result<RefreshTokenResponse>> GetRefreshTokenAsync(CancellationToken Cancel = default)
    {
        try
        {
            // TODO: Не обращай внимание, я тут буду править.
            var headersAuthorization = (string?)_ContextAccessor.HttpContext?.Request.Headers.Authorization;
            // TODO: validation
            var jwtToken = headersAuthorization.Remove(0, 7);

            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);

            var userEmail = (string)token.Payload.First(x => x.Key.Equals("email")).Value;
            // TODO: validation

            var identity_user = await _UserManager.FindByEmailAsync(userEmail);
            // TODO: validation
            var identity_roles = await _UserManager.GetRolesAsync(identity_user);
            var new_session_token = _authUtilits.CreateSessionToken(identity_user, identity_roles);

            if (!string.IsNullOrEmpty(new_session_token))
            {
                return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(new_session_token));
            }

            _Logger.Log(LogLevel.Information, "Не удалось обновить токен пользователя");
            return Result<RefreshTokenResponse>.Failure(Errors.Identity.GetRefreshToken.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Не удалось обновить токен пользователя {Ex}", ex);
            return Result<RefreshTokenResponse>.Failure(Errors.Identity.GetRefreshToken.Unhandled);
        }
    }


    /// <summary>Метод создания роли для пользователя</summary>
    /// <param name="CreateRoleRequest">Запроc на создание роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<Result> CreateRoleAsync([FromBody] CreateRoleRequest CreateRoleRequest, CancellationToken Cancel = default)
    {
        try
        {
            var identity_result = await _RoleManager.CreateAsync(new IdentityRole(CreateRoleRequest.RoleName.ToLower()));
            if (identity_result.Succeeded)
            {
                return Result.Success();
            }

            _Logger.Log(LogLevel.Information, "Не удалось создать роль");
            return Result.Failure(Errors.Identity.CreateRole.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при создании роли {Ex}", ex);
            return Result.Failure(Errors.Identity.CreateRole.Unhandled);
        }
    }

    /// <summary>Метод получения всех ролей в системе</summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet(AuthApiRoute.GetAllRoles)]
    public async Task<Result<IEnumerable<AuthRole>>> GetAllRolesAsync(CancellationToken Cancel = default)
    {
        try
        {
            var identity_roles_list = await _RoleManager.Roles.ToListAsync();
            var roles = identity_roles_list
               .Select(role => new AuthRole()
               {
                   Id = role.Id,
                   RoleName = role.Name,
               })
               .ToArray();

            return Result<IEnumerable<AuthRole>>.Success(roles);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при запросе ролей {Ex}", ex);
            return Result<IEnumerable<AuthRole>>.Failure(Errors.Identity.GetAllRoles.Unhandled);
        }
    }

    /// <summary>Метод получения роли по идентификатору</summary>
    /// <param name="RoleId">Идентификатор роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetRoleById}/" + "{RoleId}")]
    public async Task<Result<AuthRole>> GetRoleByIdAsync([FromRoute] string RoleId, CancellationToken Cancel = default)
    {
        try
        {
            var identity_role = await _RoleManager.FindByIdAsync(RoleId);
            if (identity_role is null)
            {
                _Logger.Log(LogLevel.Information, "Не удалось получить роль");
                return Result<AuthRole>.Failure(Errors.Identity.GetRoleById.NotFound);
            }

            var role = new AuthRole { Id = identity_role.Id, RoleName = identity_role.Name };
            return Result<AuthRole>.Success(role);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при запросе роли {Ex}", ex);
            return Result<AuthRole>.Failure(Errors.Identity.GetRoleById.Unhandled);
        }
    }

    /// <summary>Метод редактирования роли по идентификатору</summary>
    /// <param name="EditRoleRequest">Запрос на редактирование роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPut(AuthApiRoute.EditRoleNameById)]
    public async Task<Result> EditRoleNameByIdAsync([FromBody] EditRoleNameByIdRequest EditRoleRequest, CancellationToken Cancel = default)
    {
        try
        {
            var identity_role = await _RoleManager.FindByIdAsync(EditRoleRequest.RoleId);
            if (identity_role is null)
            {
                _Logger.Log(LogLevel.Warning, "Не удалось найти роль {RoleId}", EditRoleRequest.RoleId);
                return Result.Failure(Errors.Identity.EditRoleNameById.NotFound);
            }

            identity_role.Name = EditRoleRequest.RoleName.ToLower();
            identity_role.NormalizedName = EditRoleRequest.RoleName.ToUpper();

            var identity_result = await _RoleManager.UpdateAsync(identity_role);
            if (identity_result.Succeeded)
            {
                return Result.Success();
            }

            _Logger.Log(LogLevel.Information, "Не удалось изменить роль");
            return Result.Failure(Errors.Identity.EditRoleNameById.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при редактировании роли {Ex}", ex);
            return Result.Failure(Errors.Identity.EditRoleNameById.Unhandled);
        }
    }

    /// <summary>Метод удаления роли по идентификатору</summary>
    /// <param name="RoleId">Запрос на удаление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteRoleById}/" + "{RoleId}")]
    public async Task<Result> DeleteRoleByIdAsync([FromRoute] string RoleId, CancellationToken Cancel = default)
    {
        try
        {
            var identity_role = await _RoleManager.FindByIdAsync(RoleId);
            if (identity_role is null)
            {
                _Logger.Log(LogLevel.Warning, "Не удалось найти роль");
                return Result.Failure(Errors.Identity.DeleteRoleById.NotFound);
            }

            // check that user not try to delete ADMIN or USER roles
            if (!_authUtilits.CheckToDeleteAdminOrUserRoles(identity_role))
            {
                _Logger.Log(LogLevel.Warning, "Не удалось найти роль");
                return Result.Failure(Errors.Identity.DeleteRoleById.TryToDeleteSystemRoles);
            }

            var identity_result = await _RoleManager.DeleteAsync(identity_role);
            if (identity_result.Succeeded)
            {
                return Result.Success();
            }

            _Logger.Log(LogLevel.Warning, "Не удалось удалить роль");
            return Result.Failure(Errors.Identity.DeleteRoleById.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Error, "Произошла ошибка при удалении роли {Ex}", ex);
            return Result.Failure(Errors.Identity.DeleteRoleById.Unhandled);
        }
    }

    /// <summary>Метод добавления роли к пользователю</summary>
    /// <param name="AddRoleToUserRequest">Запрос на добавление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<Result> AddRoleToUserAsync([FromBody] AddRoleToUserRequest AddRoleToUserRequest, CancellationToken Cancel = default)
    {
        try
        {
            var normalized_role_name = AddRoleToUserRequest.RoleName.ToLowerInvariant();

            if (await _RoleManager.Roles.AnyAsync(x => x.Name == normalized_role_name))
            {
                _Logger.Log(LogLevel.Warning, "Роль не зарегистрированна {Role}", AddRoleToUserRequest.RoleName);
                return Result.Failure(Errors.Identity.AddRoleToUser.RoleNotFound);
            }

            var identity_user = await _UserManager.FindByEmailAsync(AddRoleToUserRequest.Email);
            if (identity_user is null)
            {
                _Logger.Log(LogLevel.Warning, "Пользователь не найден {User}", AddRoleToUserRequest.Email);
                return Result.Failure(Errors.Identity.AddRoleToUser.UserNotFound);
            }

            if (await _UserManager.IsInRoleAsync(identity_user, normalized_role_name))
            {
                _Logger.Log(LogLevel.Warning, "Пользователь {User} уже имеет данную роль {Role}", AddRoleToUserRequest.Email, AddRoleToUserRequest.RoleName);
                return Result.Failure(Errors.Identity.AddRoleToUser.UserAlreadyInRole);
            }

            var role_added_result = await _UserManager.AddToRoleAsync(identity_user, normalized_role_name);
            if (role_added_result.Succeeded)
            {
                // TODO: Schedule system to user update token when he will be signed in
                return Result.Success();
            }

            _Logger.Log(LogLevel.Warning, "Произошла ошибка при присвоении роли пользователю");
            return Result.Failure(Errors.Identity.AddRoleToUser.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Error, "Произошла ошибка при присвоении роли пользователю {Ex}", ex);
            return Result.Failure(Errors.Identity.AddRoleToUser.Unhandled);
        }
    }

    /// <summary>Метод удаления роли у пользователя</summary>
    /// <param name="Email">Почта</param>
    /// <param name="RoleName">Название роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteUserRoleByEmail}/" + "{Email}/{RoleName}")]
    public async Task<Result> DeleteUserRoleByEmailAsync([FromRoute] string Email, [FromRoute] string RoleName, CancellationToken Cancel = default)
    {
        try
        {
            var normalized_role_name = RoleName.ToLowerInvariant();
            if (await _RoleManager.Roles.AnyAsync(x => x.Name == normalized_role_name))
            {
                _Logger.Log(LogLevel.Warning, "Роль не зарегистрированна {Role}", RoleName);
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.RoleNotFound);
            }

            var identity_user = await _UserManager.FindByEmailAsync(Email);
            if (identity_user is null)
            {
                _Logger.Log(LogLevel.Warning, "Пользователь не найден {User}", Email);
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.UserNotFound);
            }

            if (!await _UserManager.IsInRoleAsync(identity_user, normalized_role_name))
            {
                _Logger.Log(LogLevel.Warning, "Пользователь {User} не имеет данную роль {Role}", Email, RoleName);
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.UserNotInRole);
            }

            // check that user not trying to remove super admin from admin role
            if (!_authUtilits.CheckToDeleteSAInRoleAdmin(identity_user, RoleName.ToLower()))
            {
                _Logger.Log(LogLevel.Warning, "Попытка понизить супер админа в должности");
                return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.TryToDownSuperAdmin);
            }

            var role_removed_result = await _UserManager.RemoveFromRoleAsync(identity_user, RoleName.ToLower());
            if (role_removed_result.Succeeded)
            {
                // TODO: Schedule system to user update token when he will be signed in
                return Result.Success();
            }

            _Logger.Log(
                LogLevel.Warning,
                "Некорректно введены данные {Email}, {RoleName}",
                Email, RoleName);
            return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Error, "Произошла ошибка при удалении роли пользователю {Ex}", ex);
            return Result.Failure(Errors.Identity.RemoveRoleFromUserByEmail.Unhandled);
        }
    }

    /// <summary>Метод получения всех ролей пользователя</summary>
    /// <param name="Email">Почта</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetAllUserRolesByEmail}/" + "{Email}")]
    public async Task<Result<IEnumerable<AuthRole>>> GetUserRolesAsync([FromRoute] string Email, CancellationToken Cancel = default)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(Email);
            if (identity_user is null)
            {
                _Logger.Log(LogLevel.Warning, "Пользователь не найден {User}", Email);
                return Result<IEnumerable<AuthRole>>.Failure(Errors.Identity.GetUserRoles.UserNotFound);
            }

            var roles = await GetUserRolesAsync(identity_user);

            return Result<IEnumerable<AuthRole>>.Success(roles);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Error, "Произошла ошибка при получении списка ролей пользователя {Ex}", ex);
            return Result<IEnumerable<AuthRole>>.Failure(Errors.Identity.GetUserRoles.Unhandled);
        }
    }

    /// <summary>Метод получения информации о пользователе</summary>
    /// <param name="Email">Почта</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet($"{AuthApiRoute.GetUserByEmail}/" + "{Email}")]
    public async Task<Result<AuthUser>> GetUserByEmailAsync([FromRoute] string Email, CancellationToken Cancel = default)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(Email);
            if (identity_user is null)
            {
                _Logger.Log(LogLevel.Information, "Не удалось получить информации о пользователе");
                return Result<AuthUser>.Failure(Errors.Identity.GetUserByEmail.NotFound);
            }

            var roles = await GetUserRolesAsync(identity_user);

            AuthUser user = new()
            {
                Id = identity_user.Id,
                Email = Email,
                UserName = identity_user.UserName,
                UserRoles = roles,
            };
            return Result<AuthUser>.Success(user);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователей {Ex}", ex);
            return Result<AuthUser>.Failure(Errors.Identity.GetUserByEmail.Unhandled);
        }
    }

    /// <summary>Метод получения всех пользователей в системе админом</summary>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpGet(AuthApiRoute.GetAllUsers)]
    public async Task<Result<IEnumerable<AuthUser>>> GetAllUsersAsync(CancellationToken Cancel = default)
    {
        try
        {
            var list_of_all_users = await _UserManager.Users.ToListAsync();
            var users = new List<AuthUser>();
            foreach (var user in list_of_all_users)
            {
                var roles = await GetUserRolesAsync(user);
                users.Add(new AuthUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    UserRoles = roles
                });
            }

            return Result<IEnumerable<AuthUser>>.Success(users);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при получении списка пользователей {Ex}", ex);
            return Result<IEnumerable<AuthUser>>.Failure(Errors.Identity.GetAllUsers.Unhandled);
        }
    }

    /// <summary>Метод редактирования информации о пользователе</summary>
    /// <param name="EditUserRequest">Запрос админа</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpPut(AuthApiRoute.EditUserByEmail)]
    public async Task<Result<EditUserNameResponse>> EditUserNameByEmailAsync([FromBody] EditUserNameByEmailRequest EditUserRequest, CancellationToken Cancel = default)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(EditUserRequest.UserEmail);
            if (identity_user is null)
            {
                _Logger.Log(LogLevel.Warning, "Не удалось найти пользователя {Email}", EditUserRequest.UserEmail);
                return Result<EditUserNameResponse>.Failure(Errors.Identity.EditUserName.NotFound);
            }

            identity_user.UserName = EditUserRequest.EditUserNickName;

            var identity_result = await _UserManager.UpdateAsync(identity_user);
            if (identity_result.Succeeded)
            {
                var new_token = _authUtilits.CreateSessionToken(
                    identity_user,
                    await _UserManager.GetRolesAsync(identity_user));

                return Result<EditUserNameResponse>.Success(new EditUserNameResponse(new_token));
            }

            _Logger.Log(LogLevel.Information, "Не удалось обновить информацию пользователя");
            return Result<EditUserNameResponse>.Failure(Errors.Identity.EditUserName.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при изменении имени пользователя {Ex}", ex);
            return Result<EditUserNameResponse>.Failure(Errors.Identity.EditUserName.Unhandled);
        }
    }

    /// <summary>Метод удаления пользователя</summary>
    /// <param name="Email">Почта пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = AuthApiRoles.Admin)]
    [HttpDelete($"{AuthApiRoute.DeleteUserByEmail}/" + "{Email}")]
    public async Task<Result> DeleteUserByEmailAsync([FromRoute] string Email, CancellationToken Cancel = default)
    {
        try
        {
            var identity_user = await _UserManager.FindByEmailAsync(Email.ToLower());
            if (identity_user is null)
            {
                _Logger.Log(LogLevel.Information, "Не удалось получить информацию о пользователе {Email}", Email);
                return Result.Failure(Errors.Identity.DeleteUser.NotFound);
            }

            if (_authUtilits.CheckToDeleteSA(identity_user))
            {
                var identity_result = await _UserManager.DeleteAsync(identity_user);
                if (identity_result.Succeeded)
                {
                    return Result.Success();
                }
            }

            _Logger.Log(LogLevel.Information, "Не удалось удалить пользователя {Email}", Email);
            return Result.Failure(Errors.Identity.DeleteUser.Fail);
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Information, "Произошла ошибка при удалении пользователя {Ex}", ex);
            return Result.Failure(Errors.Identity.DeleteUser.Unhandled);
        }
    }

    private async Task<List<AuthRole>> GetUserRolesAsync(IdentityUser IdentityUser, CancellationToken Cancel = default)
    {
        try
        {
            var user_roles = (await _UserManager.GetRolesAsync(IdentityUser)).ToHashSet();
            var roles = await _RoleManager.Roles
               .Where(x => user_roles.Contains(x.Name))
               .Select(x => new AuthRole()
                {
                    Id       = x.Id,
                    RoleName = x.Name,
                })
               .ToListAsync(cancellationToken: Cancel);
            return roles;
        }
        catch (Exception ex)
        {
            _Logger.Log(LogLevel.Error, "Произошла ошибка при получении списка ролей пользователей {Ex}", ex);
            throw;
        }
    }

    private Task<bool> CheckIsEmailConfirmedAsync(IdentityUser identityUser) => _UserManager.IsEmailConfirmedAsync(identityUser);
}