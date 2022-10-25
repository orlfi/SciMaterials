using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.AuthRoles;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.Contracts.API.DTO.Passwords;
using SciMaterials.Contracts.API.Services.Identity;

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
    public async Task<IActionResult> RegisterAsync([FromBody] AuthUserRequest? UserRequest, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.RegisterUserAsync(UserRequest, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpPost(AuthApiRoute.Login)]
    public async Task<IActionResult> LoginAsync([FromBody] AuthUserRequest? UserRequest, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.LoginUserAsync(UserRequest, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpPost(AuthApiRoute.Logout)]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.LogoutUserAsync(cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpPost(AuthApiRoute.ChangePassword)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest? PasswordRequest, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.ChangePasswordAsync(PasswordRequest, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpPost(AuthApiRoute.CreateRole)]
    public async Task<IActionResult> CreateRoleAsync([FromBody] AuthRoleRequest? RoleRequest, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.CreateRoleAsync(RoleRequest, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpGet(AuthApiRoute.GetAllRoles)]
    public async Task<IActionResult> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.GetAllRolesAsync(cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpGet($"{AuthApiRoute.GetRoleById}"+"{roleId}")]
    public async Task<IActionResult> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        var request = new AuthRoleRequest() {RoleId = roleId};
        var response = await _identityClient.GetRoleByIdAsync(request, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpPut(AuthApiRoute.EditRoleById)]
    public async Task<IActionResult> EditRoleByIdAsync([FromBody] AuthRoleRequest? RoleRequest, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.EditRoleByIdAsync(RoleRequest, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpDelete($"{AuthApiRoute.DeleteRoleById}"+"{roleId}")]
    public async Task<IActionResult> DeleteRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        var request = new AuthRoleRequest() {RoleId = roleId};
        var response = await _identityClient.DeleteRoleByIdAsync(request, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpPost(AuthApiRoute.AddRoleToUser)]
    public async Task<IActionResult> AddRoleToUserAsync([FromBody] AuthRoleRequest? RoleRequest, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.AddRoleToUserAsync(RoleRequest, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpDelete($"{AuthApiRoute.DeleteUserRoleByEmail}"+"{email}/{roleName}")]
    public async Task<IActionResult> DeleteUserRoleByEmailAsync(string email, string roleName, CancellationToken cancellationToken = default)
    {
        var request = new AuthRoleRequest() {Email = email, RoleName = roleName};
        var response = await _identityClient.DeleteUserRoleByEmailAsync(request, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpGet($"{AuthApiRoute.ListOfUserRolesByEmail}"+"{email}")]
    public async Task<IActionResult> ListOfUserRolesByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var request = new AuthRoleRequest() {Email = email};
        var response = await _identityClient.ListOfUserRolesAsync(request, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpPost(AuthApiRoute.CreateUser)]
    public async Task<IActionResult> CreateUserAsync([FromBody] AuthUserRequest? UserRequest, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.CreateUserAsync(UserRequest, cancellationToken);
        return Ok(response.ConfirmEmail);
    }
    
    [HttpGet($"{AuthApiRoute.GetUserByEmail}"+"{email}")]
    public async Task<IActionResult> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var request = new AuthUserRequest() {Email = email};
        var response = await _identityClient.GetUserByEmailAsync(request, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpGet(AuthApiRoute.GetAllUsers)]
    public async Task<IActionResult> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.GetAllUsersAsync(cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpPut(AuthApiRoute.EditUserByEmail)]
    public async Task<IActionResult> EditUserNameByEmailAsync([FromBody] EditUserRequest? EditUserRequest, CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.EditUserNameByEmailAsync(EditUserRequest, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpDelete($"{AuthApiRoute.DeleteUserByEmail}"+"{email}")]
    public async Task<IActionResult> DeleteUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        var request = new AuthUserRequest() {Email = email};
        var response = await _identityClient.DeleteUserByEmail(request, cancellationToken);
        return Ok(response.Content);
    }
    
    [HttpDelete(AuthApiRoute.DeleteUserWithoutConfirm)]
    public async Task<IActionResult> DeleteUsersWithOutConfirmation(CancellationToken cancellationToken = default)
    {
        var response = await _identityClient.DeleteUsersWithOutConfirm(cancellationToken);
        return Ok(response.Content);
    }
}