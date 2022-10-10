using AutoMapper;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.DAL.Models;

namespace SciMaterials.Contracts.API.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, GetCategoryResponse>().ReverseMap();
        CreateMap<Category, AddEditCategoryRequest>().ReverseMap();
    }
}
