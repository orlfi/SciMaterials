using SciMaterials.Contracts.API.DTO.Categories;

namespace SciMaterials.Contracts.API.Services.Categories;

public interface ICategoryService : IService<Guid, GetCategoryResponse>, IModifyService<AddCategoryRequest, EditCategoryRequest, Guid>
{
}