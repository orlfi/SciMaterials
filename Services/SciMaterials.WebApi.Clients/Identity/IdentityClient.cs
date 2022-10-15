using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SciMaterials.Auth.Requests;
using SciMaterials.Contracts.API.Constants;

namespace SciMaterials.WebApi.Clients.Identity;

public class IdentityClient
{
    public HttpClient Http { get; }
    public ILogger<IdentityClient> Logger { get; }

    public IdentityClient(HttpClient Http, ILogger<IdentityClient> Logger)
    {
        this.Http   = Http;
        this.Logger = Logger;
    }

    public async Task<RegisterResult> RegisterAsync(string Email, string Password)
    {
        var reuest = new UserRequest
        {
            Email = Email,
            Password = Password
        };

        var response = await Http.PostAsJsonAsync(AuthApiRoute.AuthControllerName + "/" + AuthApiRoute.Register, reuest);

        var result = await response.EnsureSuccessStatusCode()
           .Content
           .ReadFromJsonAsync<RegisterResult>();

        return result!;

    }

    public record RegisterResult(string Message, string ConfirmationEmail);
}
