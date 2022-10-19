using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.DTO.Authors;
using SciMaterials.Contracts.Result;
using System.Net.Http.Json;

namespace SciMaterials.WebApi.Clients;

public abstract class ApiClientWithAddBase<TClient, TAddRequest, TEditRequest, TId, TResponse> : ApiClientBase<TClient, TEditRequest, TId, TResponse>
{
    public ApiClientWithAddBase(HttpClient httpClient, ILogger<TClient> logger) : base(httpClient, logger)
    { }

    public virtual async Task<Result<TId>> AddAsync(TAddRequest request, CancellationToken cancellationToken = default)
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
