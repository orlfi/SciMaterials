using SciMaterials.Contracts.API.DTO.Tags;

namespace SciMaterials.Contracts.WebApi.Clients.Tags;

public interface ITagsClient :
    IApiReadonlyClient<Guid>,
    IApiModifyClient<Guid>,
    IApiDeleteClient<Guid>
{
}