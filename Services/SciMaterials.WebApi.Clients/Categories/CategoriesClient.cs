using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.WebApi.Clients.Categories;
using System.Net.Http.Json;

namespace SciMaterials.WebApi.Clients.Categories;

public class CategoriesClient :
    ApiModifiedClientWithAddBase<Guid, GetCategoryResponse, AddCategoryRequest, EditCategoryRequest>,
    ICategoriesClient
{
    public CategoriesClient(HttpClient httpClient, ILogger<CategoriesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Categories;

    public async Task<Result<CategoryTree>> GetTreeAsync(Guid? id, CancellationToken Cancel = default)
    {
        _logger.LogInformation("Get categories tree");

        var result = await _httpClient.GetFromJsonAsync<Result<CategoryTree>>(_webApiRoute, Cancel)
            ?? throw new InvalidOperationException("No response received from the server.");
        return result;
    }
}
