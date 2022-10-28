using System.Net;
using System.Net.Http.Headers;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

namespace SciMaterials.UI.BWASM.Extensions;

// TODO: add memory cache and polly retry
public class JwtAuthenticationHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _stateProvider;
    private readonly ILocalStorageService _localStorageService;

    public JwtAuthenticationHandler(AuthenticationStateProvider stateProvider, ILocalStorageService localStorageService)
    {
        _stateProvider = stateProvider;
        _localStorageService = localStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        var currentState = await _stateProvider.GetAuthenticationStateAsync();
        if (currentState.User.Identity?.IsAuthenticated is not true)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var token = await _localStorageService.GetItemAsStringAsync("authToken");

        if (request.Headers.Authorization?.Parameter != token)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}