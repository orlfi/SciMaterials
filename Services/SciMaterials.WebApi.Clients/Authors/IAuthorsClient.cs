using SciMaterials.Contracts.API.DTO.Authors;

namespace SciMaterials.WebApi.Clients.Authors;

public interface IAuthorsClient :
    IApiReadonlyClient<Guid, GetAuthorResponse>,
    IApiModifyClient<AddAuthorRequest, EditAuthorRequest, Guid>,
    IApiDeleteClient<Guid>
{
}