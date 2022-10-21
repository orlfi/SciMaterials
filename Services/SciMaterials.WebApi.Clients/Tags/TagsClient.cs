using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.WebApi.Clients.Tags;

namespace SciMaterials.WebApi.Clients.Tags;

public class TagsClient :
    ApiClientWithAddBase<TagsClient, Guid>,
    ITagsClient
{
    public TagsClient(HttpClient httpClient, ILogger<TagsClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Tags;
}
