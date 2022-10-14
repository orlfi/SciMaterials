using AutoMapper;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.DAL.Models;

namespace SciMaterials.Contracts.API.Mappings;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<Tag, GetTagResponse>().ReverseMap();
        CreateMap<Tag, AddTagRequest>().ReverseMap();
        CreateMap<Tag, EditTagRequest>().ReverseMap();
    }
}
 