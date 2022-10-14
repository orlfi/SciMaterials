using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciMaterials.Auth.Requests;
using SciMaterials.Auth.Utilits;
using SciMaterials.Contracts.API.Constants;

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
    /// <param name="registerUserRequest">Запрос пользователя</param>
    /// <returns>IActionResult</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Register)]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRequest registerUserRequest)
    {
        if (!string.IsNullOrEmpty(registerUserRequest.Email) ||
            !string.IsNullOrEmpty(registerUserRequest.Password) ||
            !string.IsNullOrEmpty(registerUserRequest.PhoneNumber))
        {
            try
            {
                var identityUser = new IdentityUser()
                {
                    Email = registerUserRequest.Email,
                    UserName = registerUserRequest.Email,
                    PhoneNumber = registerUserRequest.PhoneNumber,
                };

                var identityResult = await _userManager.CreateAsync(identityUser, registerUserRequest.Password);
                if (identityResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(identityUser, "user");
                    await _signInManager.SignInAsync(identityUser, false);
            
                    var emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = identityUser.Id, confirmToken = emailConfirmToken }, protocol: 
                        HttpContext.Request.Scheme);

                    //TODO: В будущем сделать интеграцию по отправке email для подтвреждения
                    //return Ok($"Пройдите по ссылке, чтобы подтвердить ваш email: {callbackUrl}");

                    var confirmation_token = await _userManager.GenerateEmailConfirmationTokenAsync(identityResult);

                    return Ok(new
                    {
                        Message = $"Пройдите по ссылке, чтобы подтвердить ваш email: {callbackUrl}",
                        ConfirmationEmail = Url.Action(nameof(ConfirmEmailAsync), new
                        {
                            UserId       = identityUser.Id,
                            confirmToken = confirmation_token,
                        })
                    });
                }
            
                _logger.Log(LogLevel.Information, "Не удалось зарегистрировать пользователя {Email}", 
                    registerUserRequest.Email);
                return BadRequest($"Не удалось зарегистрировать пользователя");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Пользователя не удалось зарегистрировать {Ex}", ex);
                return BadRequest("Пользователя не удалось зарегистрировать");
            }
        }
        
        _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {Password}, {PhoneNumber}", 
            registerUserRequest.Email, registerUserRequest.Password, registerUserRequest.PhoneNumber);
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод авторизации пользователя
    /// </summary>
    /// <param name="email">Почта</param>
    /// <param name="password">Пароль</param>
    /// <returns>IActionResult</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Login)]
    public async Task<IActionResult> LoginAsync(string email, string password)
    {
        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(password))
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser is not null)
                {
                    var identityRoles = await _userManager.GetRolesAsync(identityUser);
                
                    var signInResult = await _signInManager.PasswordSignInAsync(
                        userName: email, 
                        password: password, 
                        isPersistent: true, 
                        lockoutOnFailure: false);
                
                    if (signInResult.Succeeded)
                    {
                        var sessionToken = _authUtils.CreateSessionToken(identityUser, identityRoles);
                        return Ok($"Ваш токен сессии: {sessionToken}");
                    }

                    _logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Email}", email);
                    return BadRequest("Не удалось авторизовать пользователя");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Не удалось авторизовать пользователя {Ex}", ex);
                return BadRequest("Не удалось авторизовать пользователя");
            }
        }
        
        _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {Password}", 
            email, password);
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод выхода пользователя из системы
    /// </summary>
    /// <returns>IActionResult</returns>
    [AllowAnonymous]
    [HttpPost(AuthApiRoute.Logout)]
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            var currentUserName = _contextAccessor.HttpContext?.User.Identity?.Name;
            await _signInManager.SignOutAsync();
            return Ok("Пользователь вышел из системы");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Information, "Не удалось выйти из системы {Ex}", ex);
            return BadRequest("Не удалось выйти из системы!");
        }
    }
    
    /// <summary>
    /// Метод смены пароля пользователя
    /// </summary>
    /// <param name="oldPassword">Старый пароль</param>
    /// <param name="newPassword">Новый пароль</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin, user")]
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<IActionResult> ChangePasswordAsync(string oldPassword, string newPassword)
    {
        if (!string.IsNullOrEmpty(oldPassword) || !string.IsNullOrEmpty(newPassword))
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
                        identityUser, 
                        oldPassword, 
                        newPassword);
                    if (identityResult.Succeeded)
                    {
                        return Ok("Пароль пользователя успешно изменен");
                    }
                    
                    _logger.Log(LogLevel.Information, "Не удалось изменить пароль {OldPassword}, {NewPassword}", 
                        oldPassword, newPassword);
                    return BadRequest("Не удалось изменить пароль");
                }

                _logger.Log(LogLevel.Information,
                    "Не удалось получить информацию о пользователе {IdentityUser} или ваша почта не подтверждена {isEmailCorfirmed}", 
                    identityUser, isEmailConfirmed);
                return BadRequest("Не удалось получить информацию о пользователе или ваша почта не подтверждена");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при смене пароля {Ex}", ex);
                return BadRequest("Произошла ошибка при смене пароля");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {OldPasswoprd}, {NewPassword}", 
            oldPassword, newPassword);
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод подтверждения почты пользователя, когда пользователь проходит по сформированной ссылке
    /// </summary>
    /// <param name="userId">Идентификатор пользователя в системе</param>
    /// <param name="confirmToken">Токен подтверждения</param>
    /// <returns>IActionResult</returns>
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
                    return BadRequest("Не удалось подтвердить email пользователя");
                }
                
                _logger.Log(LogLevel.Information, "Не удалось найти пользователя в системе {UserId}, {ConfirmTokin}", 
                    userId, confirmToken);
                return BadRequest("Не удалось найти пользователя в системе");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при подтверждении почты {Ex}", ex);
                return BadRequest("Произошла ошибка при подтверждении почты");
            }
        }
        
        _logger.Log(LogLevel.Information, "Некорректные данные {UserId}, {ConfirmTokin}", 
            userId, confirmToken);
        return BadRequest("Некорректные данные");
    }

    /// <summary>
    /// Метод создания роли для пользователя
    /// </summary>
    /// <param name="roleName">Название роли</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<IActionResult> CreateRoleAsync(string roleName)
    {
        if (!string.IsNullOrEmpty(roleName))
        {
            try
            {
                var identityResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (identityResult.Succeeded)
                {
                    return Ok("Роль для пользователя успешно создана");
                }
                
                _logger.Log(LogLevel.Information, "Не удалось создать роль");
                return BadRequest("Не удалось создать роль");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при создании роли {Ex}", ex);
                return BadRequest("Произошла ошибка при создании роли");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {RoleName}", roleName);
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод получения всех ролей в системе
    /// </summary>
    /// <returns>IActionResult</returns>
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
            return BadRequest("Не удалось получить список ролей");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Information, "Произошла ошибка при запросе ролей {Ex}", ex);
            return BadRequest("Произошла ошибка при запросе ролей");
        }
    }
    
    /// <summary>
    /// Метод получения роли по идентификатору
    /// </summary>
    /// <param name="roleId">Идентификатор роли (не пользователя)</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpGet(AuthApiRoute.GetRoleById)]
    public async Task<IActionResult> GetRoleByIdAsync(string roleId)
    {
        if (!string.IsNullOrEmpty(roleId))
        {
            try
            {
                var identityRole = await _roleManager.FindByIdAsync(roleId);
                if (identityRole is not null)
                {
                    return Ok(identityRole);
                }

                _logger.Log(LogLevel.Information, "Не удалось получить роль");
                return BadRequest("Не удалось получить роль");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при запросе роли {Ex}", ex);
                return BadRequest("Произошла ошибка при запросе роли");
            }
        }
        
        _logger.Log(LogLevel.Information, "Некорректно введены данные {RoleId}", roleId);
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод редактирования роли по идентификатору
    /// </summary>
    /// <param name="roleId">Идентификатор роли</param>
    /// <param name="roleName">Название роли</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.EditRoleById)]
    public async Task<IActionResult> EditRoleByIdAsync(string roleId, string roleName)
    {
        //Это временная проверка
        if (!string.IsNullOrEmpty(roleId) || !string.IsNullOrEmpty(roleName))
        {
            try
            {
                var foundRole = await _roleManager.FindByIdAsync(roleId);
                foundRole.Name = roleName;
            
                var identityResult = await _roleManager.UpdateAsync(foundRole);
                if (identityResult.Succeeded)
                {
                    return Ok($"Роль успешно изменена с {foundRole} на {roleName}");
                }
                
                _logger.Log(LogLevel.Information, "Не удалось изменить роль");
                return BadRequest("Не удалось изменить роль");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при редактировании роли {Ex}", ex);
                return BadRequest("Произошла ошибка при редактировании роли");
            }
        }
        
        _logger.Log(LogLevel.Information, "Некорректно введены данные {RoleId}, {RoleName}", 
            roleId, roleName);
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод удаления роли по идентификатору
    /// </summary>
    /// <param name="roleId">Идентификатор роли</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteRoleById)]
    public async Task<IActionResult> DeleteRoleByIdAsync(string roleId)
    {
        if (!string.IsNullOrEmpty(roleId))
        {
            try
            {
                var identityRole = await _roleManager.FindByIdAsync(roleId);
                if (identityRole is not null)
                {
                    var identityResult = await _roleManager.DeleteAsync(identityRole);
                    if (identityResult.Succeeded)
                    {
                        return Ok("Роль успешно удалена");
                    }
                    
                    _logger.Log(LogLevel.Information, "Не удалось удалить роль");
                    return BadRequest("Не удалось удалить роль");
                }
                
                _logger.Log(LogLevel.Information, "Не удалось найти роль");
                return BadRequest("Не удалось найти роль");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при удалении роли {Ex}", ex);
                return BadRequest("Произошла ошибка при удалении роли");
            }
        }
        
        _logger.Log(LogLevel.Information, "Некорректно введены данные");
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод добавления роли к пользователю
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <param name="roleName">Название роли</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<IActionResult> AddRoleToUserAsync(string email, string roleName)
    {
        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(roleName))
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                var userRolesList = await _userManager.GetRolesAsync(identityUser);
                var systemRolesList = await _roleManager.Roles.ToListAsync();
                if (!userRolesList.Contains(roleName))
                {
                    var isRoleContainsInSystem = systemRolesList.Select(x => 
                        x.Name.Contains(roleName.ToLower()));
                    foreach (var isRole in isRoleContainsInSystem)
                    {
                        if (isRole)
                        {
                            var roleAddedResult = await _userManager.AddToRoleAsync(identityUser, roleName.ToLower());
                            if (roleAddedResult.Succeeded)
                            {
                                return Ok(roleAddedResult);
                            }
                        }
                    }
                }

                _logger.Log(LogLevel.Information, "Некорректно введены данные");
                return BadRequest("Некорректно введены данные");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при добавлении роли к пользователю {Ex}", ex);
                return BadRequest("Произошла ошибка при добавлении роли пользователю");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {RoleName}", 
            email, roleName);
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод удаления роли у пользователя
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <param name="roleName">Название роли</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteUserRole)]
    public async Task<IActionResult> DeleteUserRoleAsync(string email, string roleName)
    {
        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(roleName))
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                var userRolesList = await _userManager.GetRolesAsync(identityUser);
                var systemRolesList = await _roleManager.Roles.ToListAsync();
                if (userRolesList.Contains(roleName))
                {
                    var isRoleContainsInSystem = systemRolesList.Select(x => 
                        x.Name.Contains(roleName));
                    foreach (var isRole in isRoleContainsInSystem)
                    {
                        if (isRole)
                        {
                            var roleRemovedResult = await _userManager.RemoveFromRoleAsync(identityUser, roleName);
                            if (roleRemovedResult.Succeeded)
                            {
                                return Ok(roleRemovedResult);
                            }
                        }
                    }
                }

                _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}, {RoleName}", 
                    email, roleName);
                return BadRequest("Некорректно введены данные");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return BadRequest("Произошла ошибка при удалении роли пользователю");
            }
        }

        return BadRequest("Некорректно введены данные");
    }

    /// <summary>
    /// Метод получения всех ролей пользователя
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpGet(AuthApiRoute.ListOfUserRoles)]
    public async Task<IActionResult> ListOfUserRolesAsync(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser is not null)
                {
                    var userRolesList = await _userManager.GetRolesAsync(identityUser);
                    if (userRolesList.Count != 0)
                    {
                        return Ok(userRolesList.ToList());
                    }

                    _logger.Log(LogLevel.Information, "Не удалось получить список ролей");
                    return BadRequest("Не удалось получить список ролей");
                }

                _logger.Log(LogLevel.Information, 
                    "Данного пользователя {IdentityUser} нет в системе, либо некорректно введены данные пользователя " +
                    "{Email}", identityUser, email);
                return BadRequest("Данного пользователя нет в системе, либо некорректно введены данные пользователя");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при получении списка ролей пользователей {Ex}", ex);
                return BadRequest("Произошла ошибка при получении списка ролей пользователей");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}", email);
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод создания пользователя админом
    /// </summary>
    /// <param name="create">Запрос админа</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.CreateUser)]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserRequest create)
    {
        if (!string.IsNullOrEmpty(create.Email) || 
            !string.IsNullOrEmpty(create.Password) || 
            !string.IsNullOrEmpty(create.PhoneNumber))
        {
            var actionResult = await RegisterAsync(create);
            return Ok(actionResult);
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    /// <summary>
    /// Метод получения информации о пользователе
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.GetUserByEmail)]
    public async Task<IActionResult> GetUserByEmailAsync(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser is not null)
                {
                    return Ok(identityUser);
                }

                _logger.Log(LogLevel.Information, "Не удалось получить информации о пользователе");
                return BadRequest("Не удалось получить информации о пользователе");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при получении пользователей {Ex}", ex);
                return BadRequest("Произошла ошибка при получении пользователей");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные {Email}", email);
        return BadRequest("Некорректно введены данные");
    }
    
    /// <summary>
    /// Метод получения всех пользователей в системе админом (на свой страх и риск >_<
    /// </summary>
    /// <returns>IActionResult</returns>
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
            return BadRequest("Произошла ошибка при получении пользователя");
        }
    }
    
    /// <summary>
    /// Метод редактирования информации о пользователе
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <param name="editUserRequest">Запрос админа</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpPost(AuthApiRoute.EditUserByEmail)]
    public async Task<IActionResult> EditUserByEmailAsync(string email, UserRequest editUserRequest)
    {
        if (!string.IsNullOrEmpty(email) || editUserRequest is not null)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser is not null)
                {
                    identityUser.Email = editUserRequest.Email;
                    identityUser.PhoneNumber = editUserRequest.PhoneNumber;
                    identityUser.UserName = editUserRequest.Email;

                    var identityResult = await _userManager.UpdateAsync(identityUser);
                    if (identityResult.Succeeded)
                    {
                        return Ok("Информаци о пользователе успешно изменена");
                    }

                    _logger.Log(LogLevel.Information, "Не удалось обновить информацию пользователя");
                    return BadRequest("Не удалось обновить информацию пользователя");
                }

                _logger.Log(LogLevel.Information, 
                    "Не удалось найти пользователя {Email} или некорректно введены данные", email);
                return BadRequest("Не удалось найти данного пользователя или некорректно введены данные пользователя");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при обновлении информации о пользователе {Ex}", ex);
                return BadRequest("Произошла ошибка при обновлении информации о пользователе");
            }
        }
        
        _logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
        return BadRequest("Некорректно введены данные пользователя");
    }
    
    /// <summary>
    /// Метод удаления пользователя
    /// </summary>
    /// <param name="email">Почта пользователя</param>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteUserByEmail)]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                if (identityUser is not null)
                {
                    var identityResult = await _userManager.DeleteAsync(identityUser);
                    if (identityResult.Succeeded)
                    {
                        return Ok("Пользователь успешно удален");
                    }
                    
                    _logger.Log(LogLevel.Information, "Не удалось удалить пользователя {Email}", email);
                    return BadRequest("Не удалось удалить пользователя");
                }

                _logger.Log(LogLevel.Information, "Не удалось получить информацию о пользователе {Email}", email);
                return BadRequest("Не удалось получить информацию о пользователе");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, "Произошла ошибка при удалении пользователя {Ex}", ex);
                return BadRequest("Произошла ошибка при удалении пользователя");
            }
        }

        _logger.Log(LogLevel.Information, "Некорректно введены данные пользователя");
        return BadRequest("Некорректно введены данные пользователя");
    }

    /// <summary>
    /// Метод удаления пользователей без регистрации (для очистки БД)
    /// </summary>
    /// <returns>IActionResult</returns>
    [Authorize(Roles = "admin")]
    [HttpDelete(AuthApiRoute.DeleteUserWithoutConfirmation)]
    public async Task<IActionResult> DeleteUsersWithOutConfirmation()
    {
        try
        {
            var usersWithOutConfirmationEmail = await _userManager
                .Users.Where(x => 
                    x.EmailConfirmed.Equals(false))
                .ToListAsync();
            foreach (var user in usersWithOutConfirmationEmail)
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
            return BadRequest("Не удалось удалить пользователей из-за ошибки");
        }
    }
}