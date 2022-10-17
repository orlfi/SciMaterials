using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.DTO.AuthUsers;
using SciMaterials.WebApi.Clients.Identity;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary>
/// Тестовый контроллер с IdentityClient
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AccountTestController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IdentityClient _identityClient;
    public AccountTestController(IHttpClientFactory clientFactory)
    {
        _clientFactory  = clientFactory;
        _identityClient = new IdentityClient(_clientFactory.CreateClient());
    }

    [HttpPost("register_request")]
    public async Task<IActionResult> RegisterRequestToAuthService([FromBody] AuthUserRequest userRequest)
    {
        var response = await _identityClient.RegisterUserAsync(userRequest);
        return Ok(response);
    }

    [HttpPost("login_request")]
    public async Task<IActionResult> LoginRequestToAuthService([FromBody] AuthUserRequest userRequest)
    {
        var response = await _identityClient.LoginUserAsync(userRequest);
        return Ok(response);
    }
        
    [HttpPost("logout_request")]
    public async Task<IActionResult> LogoutRequestToAuthService()
    {
        var response = await _identityClient.LogoutUserAsync();
        return Ok(response);
    }
}