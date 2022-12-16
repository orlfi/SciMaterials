using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.Result;

namespace SciMaterials.Contracts.WebApi.Clients.Categories;

public interface ICategoriesClient :
    IApiReadonlyClient<Guid, GetCategoryResponse>,
    IApiModifyClient<Guid, AddCategoryRequest, EditCategoryRequest>,
    IApiDeleteClient<Guid>
{
    Task<Result<CategoryTree>> GetTreeAsync(Guid? id, CancellationToken Cancel = default);
}