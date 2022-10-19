using SciMaterials.Contracts.API.DTO.Tags;

namespace SciMaterials.WebApi.Clients.Tags;

public interface ITagsClient :
    IApiReadonlyClient<Guid, GetTagResponse>,
    IApiModifyClient<AddTagRequest, EditTagRequest, Guid>,
    IApiDeleteClient<Guid>
{
}