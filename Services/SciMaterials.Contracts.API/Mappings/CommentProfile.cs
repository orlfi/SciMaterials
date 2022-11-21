using AutoMapper;
using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.DAL.Contracts.Entities;

namespace SciMaterials.Contracts.API.Mappings;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, GetCommentResponse>().ReverseMap();
        CreateMap<Comment, AddCommentRequest>().ReverseMap();
        CreateMap<Comment, EditCommentRequest>().ReverseMap();
    }
}
