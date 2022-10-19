using SciMaterials.Contracts.API.DTO.Files;
using SciMaterials.Contracts.Result;

namespace SciMaterials.WebApi.Clients;

public interface IApiModifyClient<TAddRequest, TEditRequest, TId> :
    IApiAddClient<TAddRequest, TId>,
    IApiEditClient<TAddRequest, TId>,
    IApiDeleteClient<TId>
{ }
