using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Categories;

namespace SciMaterials.WebApi.Clients.Categories;

public class CategoriesClient :
    ApiClientWithAddBase<CategoriesClient, AddCategoryRequest, EditCategoryRequest, Guid, GetCategoryResponse>,
    ICategoriesClient
{
    public CategoriesClient(HttpClient httpClient, ILogger<CategoriesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Categories;
}
