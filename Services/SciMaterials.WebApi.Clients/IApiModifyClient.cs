namespace SciMaterials.WebApi.Clients;

public interface IApiModifyClient<TAddRequest, TEditRequest, TId> :
    IApiAddClient<TAddRequest, TId>,
    IApiEditClient<TEditRequest, TId>,
    IApiDeleteClient<TId>
{ }
