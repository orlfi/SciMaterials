using SciMaterials.Contracts.API.DTO.Authors;

namespace SciMaterials.Contracts.WebApi.Clients.Authors;

public interface IAuthorsClient :
    IApiReadonlyClient<Guid>,
    IApiModifyClient<Guid>,
    IApiDeleteClient<Guid>
{
}