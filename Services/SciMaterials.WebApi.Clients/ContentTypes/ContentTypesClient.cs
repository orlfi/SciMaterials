using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.ContentTypes;

namespace SciMaterials.WebApi.Clients.ContentTypes;

public class ContentTypesClient :
    ApiClientWithAddBase<ContentTypesClient, AddContentTypeRequest, EditContentTypeRequest, Guid, GetContentTypeResponse>,
    IContentTypesClient
{
    public ContentTypesClient(HttpClient httpClient, ILogger<ContentTypesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.ContentTypes;
}
