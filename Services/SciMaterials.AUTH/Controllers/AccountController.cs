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
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<AccountController> _logger;
    private readonly IAuthUtils _authUtils;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AccountController> logger,
        IHttpContextAccessor contextAccessor,
        IAuthUtils authUtils)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
        _contextAccessor = contextAccessor;
        _authUtils = authUtils;
    }

    /// <summary>
    /// Метод регистрации пользователя
    /// </summary>
    /// <param name="userRequest">Запрос пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Register)]
    public async Task<IActionResult> RegisterAsync([FromBody] AuthUserRequest? userRequest)
    {
        if (userRequest is null)
        {
            _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {Password}",
                userRequest?.Email, userRequest?.Password);
            return Ok(new {Message = $"{ResultCodes.ValidationError}"});
        }
        
        try
        {
            var identityUser = new IdentityUser{Email = userRequest.Email, UserName = userRequest.Email};

            var identityResult = await _userManager.CreateAsync(identityUser, userRequest.Password);
            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(identityUser, AuthApiRoles.User);
                await _signInManager.SignInAsync(identityUser, false);

                var emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = identityUser.Id, confirmToken = emailConfirmToken }, protocol:
                    HttpContext.Request.Scheme);

                return Ok(new RegisterResponse
                {
                    Succeeded = true,
                    Code = (int)ResultCodes.Ok,
                    Message = "Пройдите по ссылке, чтобы подтвердить ваш email",
                    ConfirmEmail = callbackUrl,
                });
            }

            _logger.Log(LogLevel.Information, "Не удалось зарегистрировать пользователя {Email}",
                userRequest.Email);
            return Ok(new RegisterResponse(){Succeeded = false, Code = (int)ResultCodes.NotFound});
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Information, "Пользователя не удалось зарегистрировать {Ex}", ex);
            return Ok(new RegisterResponse(){Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод авторизации пользователя
    /// </summary>
    /// <param name="userRequest">Запрос пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Login)]
    public async Task<IActionResult> LoginAsync([FromBody] AuthUserRequest? userRequest)
    {
        if (userRequest is not null)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(userRequest.Email);
                if (identityUser is not null)
                {
                    var identityRoles = await _userManager.GetRolesAsync(identityUser);

                    var signInResult = await _signInManager.PasswordSignInAsync(
                        userName: userRequest.Email,
                        password: userRequest.Password,
                        isPersistent: true,
                        lockoutOnFailure: false);

                    if (signInResult.Succeeded)
                    {
                        var sessionToken = _authUtils.CreateSessionToken(identityUser, identityRoles);

                        return Ok(new LoginResponse
                        {
                            Succeeded = true,
                            Code = (int) ResultCodes.Ok,
                            Message = Response.Headers.Authorization = "Bearer " + $"{sessionToken}"
                        });
                    }

                    _logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Email}", userRequest.Email);
                    return Ok(new LoginResponse{Succeeded = false, Code = (int)ResultCodes.NotFound});
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Ex}", ex);
                return Ok(new LoginResponse{Succeeded = false, Code = (int)ResultCodes.ServerError});
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {Password}",
            userRequest?.Email, userRequest?.Password);
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
            await _signInManager.SignOutAsync();
            return Ok(new LogoutResponse{Succeeded = true, Code = (int)ResultCodes.Ok});
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Information, "Не удалось выйти из системы {Ex}", ex);
            return Ok(new LogoutResponse{Succeeded = false, Code = (int)ResultCodes.ServerError});
        }
    }

    /// <summary>
    /// Метод смены пароля пользователя
    /// </summary>
    /// <param name="passwordRequest">Запрос на смену пароля</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin, user")]
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest? passwordRequest)
    {
        if (passwordRequest is not null)
        {
            try
            {
                var currentUserName = _contextAccessor.HttpContext?.User.Identity?.Name;

                var identityUser = await _userManager.FindByNameAsync(currentUserName);
                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(identityUser);
                if (!string.IsNullOrEmpty(currentUserName) ||
                    identityUser is not null ||
                    isEmailConfirmed)
                {
                    var identityResult = await _userManager.ChangePasswordAsync(
                        identityUser!,
                        passwordRequest.CurrentPassword,
                        passwordRequest.NewPassword);
                    if (identityResult.Succeeded)
                    {
                        return Ok(await Result.SuccessAsync("Пароль пользователя успешно изменен"));
                    }

                    _logger.Log(LogLevel.Information, "Не удалось изменить пароль {CurrentPassword}, {NewPassword}",
                        passwordRequest.CurrentPassword, passwordRequest.NewPassword);
                    return Ok(await Result.SuccessAsync("Не удалось изменить пароль"));
                }

                _logger.Log(LogLevel.Information,
                    "Не удалось получить информацию о пользователе {IdentityUser} или ваша почта не подтверждена {isEmailCorfirmed}",
                    identityUser, isEmailConfirmed);
                return Ok(await Result.SuccessAsync("Не удалось получить информацию о пользователе или ваша почта не подтверждена"));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при смене пароля {Ex}", ex);
                return Ok(await Result.SuccessAsync("Произошла ошибка при смене пароля"));
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {CurrentPasswoprd}, {NewPassword}",
            passwordRequest?.CurrentPassword, passwordRequest?.NewPassword);
        return Ok(await Result.SuccessAsync("Некорректно введены данные"));
    }

    /// <summary>
    /// Метод подтверждения почты пользователя, когда пользователь проходит по сформированной ссылке
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в системе</param>
    /// <param name="confirmToken">Токен подтверждения</param>
    /// <returns>Status 200 OK.</returns>
    [HttpGet(AuthApiRoute.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string confirmToken)
    {
        if (!string.IsNullOrEmpty(userId) || !string.IsNullOrEmpty(confirmToken))
        {
            try
            {
                var identityUser = await _userManager.FindByIdAsync(userId);
                if (identityUser is not null)
                {
                    var identityResult = await _userManager.ConfirmEmailAsync(identityUser, confirmToken);
                    if (identityResult.Succeeded)
                    {
                        return Ok("Почта успешно подтверждена");
                    }

                    _logger.Log(LogLevel.Information, "Не удалось подтвердить email пользователя");
                    return Ok("Не удалось подтвердить email пользователя");
                }

                _logger.Log(LogLevel.Information, "Не удалось найти пользователя в системе {UserId}, {ConfirmTokin}",
                    userId, confirmToken);
                return Ok("Не удалось найти пользователя в системе");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при подтверждении почты {Ex}", ex);
                return Ok("Произошла ошибка при подтверждении почты");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректные данные {UserId}, {ConfirmTokin}",
            userId, confirmToken);
        return Ok("Некорректные данные");
    }

    /// <summary>
    /// Метод создания роли для пользователя
    /// </summary>
    /// <param name="roleRequest">Запро на создание роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<IActionResult> CreateRoleAsync([FromBody] AuthRoleRequest? roleRequest)
    {
        if (roleRequest is not null)
        {
            try
            {
                var identityResult = await _roleManager.CreateAsync(new IdentityRole(roleRequest.RoleName));
                if (identityResult.Succeeded)
                {
                    return Ok("Роль для пользователя успешно создана");
                }

                _logger.Log(LogLevel.Information, "Не удалось создать роль");
                return Ok("Не удалось создать роль");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при создании роли {Ex}", ex);
                return Ok("Произошла ошибка при создании роли");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {RoleName}", roleRequest);
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
            var identityRolesList = await _roleManager.Roles.ToListAsync();
            if (identityRolesList.Count != 0)
            {
                return Ok(identityRolesList);
            }

            _logger.Log(LogLevel.Information, "Не удалось получить список ролей");
            return Ok("Не удалось получить список ролей");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Information, "Произошла ошибка при запросе ролей {Ex}", ex);
            return Ok("Произошла ошибка при запросе ролей");
        }
    }

    /// <summary>
    /// Метод получения роли по идентификатору
    /// </summary>
    /// <param name="roleRequest">Запрос на получение информации о роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpGet(AuthApiRoute.GetRoleById)]
    public async Task<IActionResult> GetRoleByIdAsync([FromBody] AuthRoleRequest? roleRequest)
    {
        if (roleRequest is not null)
        {
            try
            {
                var identityRole = await _roleManager.FindByIdAsync(roleRequest.RoleId);
                if (identityRole is not null)
                {
                    return Ok(identityRole);
                }

                _logger.Log(LogLevel.Information, "Не удалось получить роль");
                return Ok("Не удалось получить роль");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при запросе роли {Ex}", ex);
                return Ok("Произошла ошибка при запросе роли");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {RoleId}", roleRequest?.RoleId);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод редактирования роли по идентификатору
    /// </summary>
    /// <param name="roleRequest">Запрос на редактирование роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.EditRoleById)]
    public async Task<IActionResult> EditRoleByIdAsync([FromBody] AuthRoleRequest? roleRequest)
    {
        if (roleRequest is not null)
        {
            try
            {
                var foundRole = await _roleManager.FindByIdAsync(roleRequest.RoleId);
                foundRole.Name = roleRequest.RoleName;

                var identityResult = await _roleManager.UpdateAsync(foundRole);
                if (identityResult.Succeeded)
                {
                    return Ok($"Роль успешно изменена с {foundRole} на {roleRequest.RoleName}");
                }

                _logger.Log(LogLevel.Information, "Не удалось изменить роль");
                return Ok("Не удалось изменить роль");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при редактировании роли {Ex}", ex);
                return Ok("Произошла ошибка при редактировании роли");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {RoleId}, {RoleName}",
            roleRequest?.RoleId, roleRequest.RoleName);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод удаления роли по идентификатору
    /// </summary>
    /// <param name="roleRequest">Запрос на удаление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteRoleById)]
    public async Task<IActionResult> DeleteRoleByIdAsync([FromBody] AuthRoleRequest? roleRequest)
    {
        if (roleRequest is not null)
        {
            try
            {
                var identityRole = await _roleManager.FindByIdAsync(roleRequest.RoleId);
                if (identityRole is not null)
                {
                    var identityResult = await _roleManager.DeleteAsync(identityRole);
                    if (identityResult.Succeeded)
                    {
                        return Ok("Роль успешно удалена");
                    }

                    _logger.Log(LogLevel.Information, "Не удалось удалить роль");
                    return Ok("Не удалось удалить роль");
                }

                _logger.Log(LogLevel.Information, "Не удалось найти роль");
                return Ok("Не удалось найти роль");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при удалении роли {Ex}", ex);
                return Ok("Произошла ошибка при удалении роли");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные");
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод добавления роли к пользователю
    /// </summary>
    /// <param name="roleRequest">Запрос на добавление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<IActionResult> AddRoleToUserAsync([FromBody] AuthRoleRequest? roleRequest)
    {
        if (roleRequest is not null)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(roleRequest.Email);
                var userRolesList = await _userManager.GetRolesAsync(identityUser);
                var systemRolesList = await _roleManager.Roles.ToListAsync();
                if (!userRolesList.Contains(roleRequest.RoleName))
                {
                    var isRoleContainsInSystem = systemRolesList.Select(x =>
                        x.Name.Contains(roleRequest.RoleName!.ToLower()));
                    foreach (var isRole in isRoleContainsInSystem)
                    {
                        if (isRole)
                        {
                            var roleAddedResult = await _userManager.AddToRoleAsync(identityUser, roleRequest.RoleName!.ToLower());
                            if (roleAddedResult.Succeeded)
                            {
                                return Ok(roleAddedResult);
                            }
                        }
                    }
                }

                _logger.Log(LogLevel.Information, "Некорректно введены данные");
                return Ok("Некорректно введены данные");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при добавлении роли к пользователю {Ex}", ex);
                return Ok("Произошла ошибка при добавлении роли пользователю");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {RoleName}",
            roleRequest?.Email, roleRequest?.RoleName);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод удаления роли у пользователя
    /// </summary>
    /// <param name="roleRequest">Запрос на удаление роли</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteUserRole)]
    public async Task<IActionResult> DeleteUserRoleAsync([FromBody] AuthRoleRequest? roleRequest)
    {
        if (roleRequest is not null)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(roleRequest.Email);
                var userRolesList = await _userManager.GetRolesAsync(identityUser);
                var systemRolesList = await _roleManager.Roles.ToListAsync();
                if (userRolesList.Contains(roleRequest.RoleName))
                {
                    var isRoleContainsInSystem = systemRolesList.Select(x =>
                        x.Name.Contains(roleRequest.RoleName!));
                    foreach (var isRole in isRoleContainsInSystem)
                    {
                        if (isRole)
                        {
                            var roleRemovedResult = await _userManager.RemoveFromRoleAsync(identityUser, roleRequest.RoleName);
                            if (roleRemovedResult.Succeeded)
                            {
                                return Ok(roleRemovedResult);
                            }
                        }
                    }
                }

                _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {RoleName}",
                    roleRequest.Email, roleRequest.RoleName);
                return Ok("Некорректно введены данные");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при удалении роли пользователю {Ex}", ex);
                return Ok("Произошла ошибка при удалении роли пользователю");
            }
        }

        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод получения всех ролей пользователя
    /// </summary>
    /// <param name="roleRequest">Запрос на получение списка ролей</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpGet(AuthApiRoute.ListOfUserRoles)]
    public async Task<IActionResult> ListOfUserRolesAsync([FromBody] AuthRoleRequest? roleRequest)
    {
        if (roleRequest is not null)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(roleRequest.Email);
                if (identityUser is not null)
                {
                    var userRolesList = await _userManager.GetRolesAsync(identityUser);
                    if (userRolesList.Count != 0)
                    {
                        return Ok(userRolesList.ToList());
                    }

                    _logger.Log(LogLevel.Information, "Не удалось получить список ролей");
                    return Ok("Не удалось получить список ролей");
                }

                _logger.Log(LogLevel.Information,
                    "Данного пользователя {IdentityUser} нет в системе, либо некорректно введены данные пользователя " +
                    "{Email}", identityUser, roleRequest.Email);
                return Ok("Данного пользователя нет в системе, либо некорректно введены данные пользователя");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при получении списка ролей пользователей {Ex}", ex);
                return Ok("Произошла ошибка при получении списка ролей пользователей");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}", roleRequest?.Email);
        return Ok("Некорректно введены данные");
    }

    /// <summary>
    /// Метод создания пользователя админом
    /// </summary>
    /// <param name="userRequest">Запрос админа</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.CreateUser)]
    public async Task<IActionResult> CreateUserAsync([FromBody] AuthUserRequest? userRequest)
    {
        if (userRequest is not null)
        {
            var actionResult = await RegisterAsync(userRequest);
            return Ok(actionResult);
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
        return Ok("Некорректно введены данные пользователя");
    }

    /// <summary>
    /// Метод получения информации о пользователе
    /// </summary>
    /// <param name="userRequest">Запрос на получение информации о пользователе</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.GetUserByEmail)]
    public async Task<IActionResult> GetUserByEmailAsync([FromBody] AuthUserRequest? userRequest)
    {
        if (userRequest is not null)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(userRequest.Email);
                if (identityUser is not null)
                {
                    return Ok(identityUser);
                }

                _logger.Log(LogLevel.Information, "Не удалось получить информации о пользователе");
                return Ok("Не удалось получить информации о пользователе");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователей {Ex}", ex);
                return Ok("Произошла ошибка при получении пользователей");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}", userRequest?.Email);
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
            var listOfAllUsers = await _userManager.Users.ToListAsync();
            return Ok(listOfAllUsers);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователя {Ex}", ex);
            return Ok("Произошла ошибка при получении пользователя");
        }
    }

    /// <summary>
    /// Метод редактирования информации о пользователе
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <param name="editUserRequest">Запрос админа</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.EditUserByEmail)]
    public async Task<IActionResult> EditUserByEmailAsync([FromBody] EditUserRequest? editUserRequest)
    {
        if (editUserRequest is not null)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(editUserRequest.Email);
                if (identityUser is not null)
                {
                    identityUser.Email = editUserRequest.EditUserInfo?.Email;
                    identityUser.UserName = editUserRequest.EditUserInfo?.Email;

                    var identityResult = await _userManager.UpdateAsync(identityUser);
                    if (identityResult.Succeeded)
                    {
                        return Ok("Информаци о пользователе успешно изменена");
                    }

                    _logger.Log(LogLevel.Information, "Не удалось обновить информацию пользователя");
                    return Ok("Не удалось обновить информацию пользователя");
                }

                _logger.Log(LogLevel.Information,
                    "Не удалось найти пользователя {Email} или некорректно введены данные", editUserRequest.Email);
                return Ok("Не удалось найти данного пользователя или некорректно введены данные пользователя");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при обновлении информации о пользователе {Ex}", ex);
                return Ok("Произошла ошибка при обновлении информации о пользователе");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
        return Ok("Некорректно введены данные пользователя");
    }

    /// <summary>
    /// Метод удаления пользователя
    /// </summary>
    /// <param name="userRequest">Запрос на удаление пользователя</param>
    /// <returns>Status 200 OK.</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteUserByEmail)]
    public async Task<IActionResult> DeleteUserByEmail([FromBody] AuthUserRequest? userRequest)
    {
        if (userRequest is not null)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(userRequest.Email);
                if (identityUser is not null)
                {
                    var identityResult = await _userManager.DeleteAsync(identityUser);
                    if (identityResult.Succeeded)
                    {
                        return Ok("Пользователь успешно удален");
                    }

                    _logger.Log(LogLevel.Information, "Не удалось удалить пользователя {Email}", userRequest.Email);
                    return Ok("Не удалось удалить пользователя");
                }

                _logger.Log(LogLevel.Information, "Не удалось получить информацию о пользователе {Email}", userRequest.Email);
                return Ok("Не удалось получить информацию о пользователе");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при удалении пользователя {Ex}", ex);
                return Ok("Произошла ошибка при удалении пользователя");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
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
            var usersToDeleteList = await _userManager
                .Users.Where(x =>
                    x.EmailConfirmed.Equals(false))
                .ToListAsync();
            foreach (var user in usersToDeleteList)
            {
                if (user.EmailConfirmed is false)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            return Ok("Пользователи без регистрации успешно удалены");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Information, "Не удалось удалить пользователей из-за ошибки {Ex}", ex);
            return Ok("Не удалось удалить пользователей из-за ошибки");
        }
    }
}