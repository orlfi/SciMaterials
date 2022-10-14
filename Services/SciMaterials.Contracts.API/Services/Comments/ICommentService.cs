using SciMaterials.Contracts.API.DTO.Comments;

namespace SciMaterials.Contracts.API.Services.Comments;

public interface ICommentService : IService<Guid, GetCommentResponse>, IModifyService<AddCommentRequest, EditCommentRequest, Guid>
{
}