using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.Result;
using System.Net.Http.Json;

namespace SciMaterials.WebApi.Clients;

public abstract class ApiClientWithAddBase<TClient, TId> : ApiClientBase<TClient, TId>
{
    public ApiClientWithAddBase(HttpClient httpClient, ILogger<TClient> logger) : base(httpClient, logger)
    { }

    public virtual async Task<Result<TId>> AddAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Add entity {request}", request);

        var response = await _httpClient.PutAsJsonAsync($"{_webApiRoute}/Add", request, cancellationToken);
        var result = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<Result<TId>>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }
}
