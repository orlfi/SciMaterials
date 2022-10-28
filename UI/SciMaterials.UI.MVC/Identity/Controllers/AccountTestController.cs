using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.AuthApi.DTO.Roles;
using SciMaterials.Contracts.AuthApi.DTO.Users;
using SciMaterials.Contracts.Identity.API.DTO.Roles;
using SciMaterials.Contracts.Identity.Clients.Clients;

namespace SciMaterials.UI.MVC.Identity.Controllers;

/// <summary>
/// Тестовый контроллер с IdentityClient
/// </summary>
[Route("account_test/")]
[ApiController]
public class AccountTestController : ControllerBase
{
    private readonly IIdentityClient _identityClient;
    public AccountTestController(IIdentityClient identityClient)
    {
        _identityClient = identityClient;
    }
    
    [HttpPost(AuthApiRoute.Register)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest? RegisterRequest, 
        CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.RegisterUserAsync(RegisterRequest, cancellationToken);
        return Ok(response.ConfirmEmail);
    }
    
    [HttpPost(AuthApiRoute.Login)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest? LoginRequest, 
        CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.LoginUserAsync(LoginRequest, cancellationToken);
        return Ok(response.SessionToken);
    }
    
    [HttpPost(AuthApiRoute.Logout)]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.LogoutUserAsync(cancellationToken);
        return Ok(response.Message);
    }
    
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest? PasswordRequest, 
        CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.ChangePasswordAsync(PasswordRequest, cancellationToken);
        return Ok(response.Message);
    }
    
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<IActionResult> CreateRoleAsync(CreateRoleRequest CreateRoleRequest, 
        CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.CreateRoleAsync(CreateRoleRequest, cancellationToken);
        return Ok(response.Message);
    }
    
    [HttpGet(AuthApiRoute.GetAllRoles)]
    public async Task<IActionResult> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.GetAllRolesAsync(cancellationToken);
        return Ok(response.Roles);
    }
    
    [HttpGet($"{AuthApiRoute.GetRoleById}"+"{RoleId}")]
    public async Task<IActionResult> GetRoleByIdAsync(string RoleId, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.GetRoleByIdAsync(RoleId, cancellationToken);
        return Ok(response.Role);
    }
    
    [HttpPut(AuthApiRoute.EditRoleNameById)]
    public async Task<IActionResult> EditRoleNameByIdAsync([FromBody] EditRoleNameByIdRequest EditRoleNameByIdRequest, 
        CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.EditRoleNameByIdAsync(EditRoleNameByIdRequest, cancellationToken);
        return Ok(response.Message);
    }
    
    [HttpDelete($"{AuthApiRoute.DeleteRoleById}"+"{RoleId}")]
    public async Task<IActionResult> DeleteRoleByIdAsync(string RoleId, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.DeleteRoleByIdAsync(RoleId, cancellationToken);
        return Ok(response.Message);
    }
    
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<IActionResult> AddRoleToUserAsync([FromBody] AddRoleToUserRequest? AddRoleRequest, 
        CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.AddRoleToUserAsync(AddRoleRequest, cancellationToken);
        return Ok(response.NewToken);
    }
    
    [HttpDelete($"{AuthApiRoute.DeleteUserRoleByEmail}"+"{Email}/{RoleName}")]
    public async Task<IActionResult> DeleteUserRoleByEmailAsync(string Email, string RoleName, 
        CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.DeleteUserRoleByEmailAsync(Email, RoleName, cancellationToken);
        return Ok(response.Message);
    }
    
    [HttpGet($"{AuthApiRoute.GetAllUserRolesByEmail}"+"{Email}")]
    public async Task<IActionResult> GetAllUserRolesByEmailAsync(string Email, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.GetAllUserRolesByEmailAsync(Email, cancellationToken);
        return Ok(response.Roles);
    }
    
    [HttpPost(AuthApiRoute.CreateUser)]
    public async Task<IActionResult> CreateUserAsync([FromBody] RegisterRequest? CreateUserRequest, 
        CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.CreateUserAsync(CreateUserRequest, cancellationToken);
        return Ok(response.ConfirmEmail);
    }
    
    [HttpGet($"{AuthApiRoute.GetUserByEmail}"+"{Email}")]
    public async Task<IActionResult> GetUserByEmailAsync(string Email, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.GetUserByEmailAsync(Email, cancellationToken);
        return Ok(response.User);
    }
    
    [HttpGet(AuthApiRoute.GetAllUsers)]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.GetAllUsersAsync(cancellationToken);
        return Ok(response.Users);
    }
    
    [HttpPut(AuthApiRoute.EditUserByEmail)]
    public async Task<IActionResult> EditUserNameByEmailAsync([FromBody] EditUserNameByEmailRequest? EditUserRequest, 
        CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.EditUserNameByEmailAsync(EditUserRequest, cancellationToken);
        return Ok(response.NewToken);
    }
    
    [HttpDelete($"{AuthApiRoute.DeleteUserByEmail}"+"{Email}")]
    public async Task<IActionResult> DeleteUserByEmailAsync(string Email, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.DeleteUserByEmailAsync(Email, cancellationToken);
        return Ok(response.Message);
    }
    
    [HttpDelete(AuthApiRoute.DeleteUserWithoutConfirm)]
    public async Task<IActionResult> DeleteUsersWithOutConfirmAsync(CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.DeleteUsersWithOutConfirmAsync(cancellationToken);
        return Ok(response.Message);
    }

    [HttpGet(AuthApiRoute.RefreshToken)]
    public async Task<IActionResult> GetRefreshTokenAsync(CancellationToken cancellationToken)
    {
        var response = await _identityClient.GetRefreshTokenAsync(cancellationToken);
        return Ok(response);
    }
}