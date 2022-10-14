using AutoMapper;
using SciMaterials.Contracts.API.DTO.Authors;
using SciMaterials.DAL.Models;

namespace SciMaterials.Contracts.API.Mappings;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<Author, GetAuthorResponse>().ReverseMap();
        CreateMap<Author, AddAuthorRequest>().ReverseMap();
        CreateMap<Author, EditAuthorRequest>().ReverseMap();
    }
}
