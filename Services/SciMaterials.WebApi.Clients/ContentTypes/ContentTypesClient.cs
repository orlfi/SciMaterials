using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.WebApi.Clients.ContentTypes;

namespace SciMaterials.WebApi.Clients.ContentTypes;

public class ContentTypesClient :
    ApiClientWithAddBase<ContentTypesClient, Guid>,
    IContentTypesClient
{
    public ContentTypesClient(HttpClient httpClient, ILogger<ContentTypesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.ContentTypes;
}
