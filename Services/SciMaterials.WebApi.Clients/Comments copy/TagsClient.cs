using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Tags;

namespace SciMaterials.WebApi.Clients.Tags;

public class TagsClient :
    ApiClientWithAddBase<TagsClient, AddTagRequest, EditTagRequest, Guid, GetTagResponse>,
    ITagsClient
{
    public TagsClient(HttpClient httpClient, ILogger<TagsClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Tags;
}
