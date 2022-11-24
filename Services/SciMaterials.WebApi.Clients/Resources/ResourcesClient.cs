using Microsoft.Extensions.Logging;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.WebApi.Clients.Tags;

public class ResourcesClient :
    ApiReadonlyClientBase<Guid, GetTagResponse>,
    IResourcesClient
{
    public ResourcesClient(HttpClient httpClient, ILogger<ResourcesClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Tags;
}
