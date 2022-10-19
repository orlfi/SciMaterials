using SciMaterials.Contracts.API.DTO.Categories;

namespace SciMaterials.WebApi.Clients.Categories;

public interface ICategoriesClient :
    IApiReadonlyClient<Guid, GetCategoryResponse>,
    IApiModifyClient<AddCategoryRequest, EditCategoryRequest, Guid>,
    IApiDeleteClient<Guid>
{
}