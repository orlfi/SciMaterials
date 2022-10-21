namespace SciMaterials.Contracts.WebApi.Clients;

public interface IApiModifyClient<TId> :
    IApiAddClient<TId>,
    IApiEditClient<TId>,
    IApiDeleteClient<TId>
{ }
