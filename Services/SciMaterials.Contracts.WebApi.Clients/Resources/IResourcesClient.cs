using SciMaterials.Contracts.API.DTO.Tags;

namespace SciMaterials.Contracts.WebApi.Clients.Tags;

public interface IResourcesClient : IApiReadonlyClient<Guid, GetTagResponse>
{
}