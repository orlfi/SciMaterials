using SciMaterials.Contracts.API.DTO.ContentTypes;

namespace SciMaterials.WebApi.Clients.ContentTypes;

public interface IContentTypesClient :
    IApiReadonlyClient<Guid, GetContentTypeResponse>,
    IApiModifyClient<AddContentTypeRequest, EditContentTypeRequest, Guid>,
    IApiDeleteClient<Guid>
{
}