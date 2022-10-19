using Microsoft.Extensions.Logging;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Comments;

namespace SciMaterials.WebApi.Clients.Comments;

public class CommentsClient :
    ApiClientWithAddBase<CommentsClient, AddCommentRequest, EditCommentRequest, Guid, GetCommentResponse>,
    ICommentsClient
{
    public CommentsClient(HttpClient httpClient, ILogger<CommentsClient> logger) : base(httpClient, logger)
        => _webApiRoute = WebApiRoute.Comments;
}
