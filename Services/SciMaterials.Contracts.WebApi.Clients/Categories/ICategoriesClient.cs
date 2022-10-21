using SciMaterials.Contracts.API.DTO.Categories;

namespace SciMaterials.Contracts.WebApi.Clients.Categories;

public interface ICategoriesClient :
    IApiReadonlyClient<Guid>,
    IApiModifyClient<Guid>,
    IApiDeleteClient<Guid>
{
}