using AutoMapper;
using SciMaterials.DAL.Models;
using SciMaterials.UI.MVC.API.DTO.Categories;

namespace SciMaterials.UI.MVC.API.Mappings;

public class CategoryProfile:Profile
{
	public CategoryProfile()
	{
		CreateMap<Category, GetCategoryResponse>().ReverseMap();
        CreateMap<Category, AddEditCategoryRequest>().ReverseMap();
    }
}
