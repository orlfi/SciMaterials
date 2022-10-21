using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.WebApi.Clients.Categories;

namespace SciMaterials.WebApi.Clients.Categories;

public class CategoriesClient :
    ApiClientWithAddBase<CategoriesClient, Guid>,
    ICategoriesClient
{
    public CategoriesClient(HttpClient httpClient, ILogger<CategoriesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Categories;
}
