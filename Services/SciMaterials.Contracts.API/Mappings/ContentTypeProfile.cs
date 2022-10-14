using AutoMapper;
using SciMaterials.Contracts.API.DTO.ContentTypes;
using SciMaterials.DAL.Models;

namespace SciMaterials.Contracts.API.Mappings;

public class ContentTypeProfile : Profile
{
    public ContentTypeProfile()
    {
        CreateMap<ContentType, GetContentTypeResponse>().ReverseMap();
        CreateMap<ContentType, AddContentTypeRequest>().ReverseMap();
        CreateMap<ContentType, EditContentTypeRequest>().ReverseMap();
    }
}
