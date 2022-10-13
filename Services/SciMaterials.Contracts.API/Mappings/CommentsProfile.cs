using AutoMapper;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.API.DTO.Comments;
using SciMaterials.DAL.Models;

namespace SciMaterials.Contracts.API.Mappings;

public class CommentsProfile : Profile
{
    public CommentsProfile()
    {
        CreateMap<Comment, CommentEditRequest>().ReverseMap();
    }
}
