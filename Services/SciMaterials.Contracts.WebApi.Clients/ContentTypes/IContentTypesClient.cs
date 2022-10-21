using SciMaterials.Contracts.API.DTO.ContentTypes;

namespace SciMaterials.Contracts.WebApi.Clients.ContentTypes;

public interface IContentTypesClient :
    IApiReadonlyClient<Guid>,
    IApiModifyClient<Guid>,
    IApiDeleteClient<Guid>
{
}