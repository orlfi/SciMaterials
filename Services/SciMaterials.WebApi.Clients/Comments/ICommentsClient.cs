using SciMaterials.Contracts.API.DTO.Comments;

namespace SciMaterials.WebApi.Clients.Comments;

public interface ICommentsClient :
    IApiReadonlyClient<Guid, GetCommentResponse>,
    IApiModifyClient<AddCommentRequest, EditCommentRequest, Guid>,
    IApiDeleteClient<Guid>
{
}