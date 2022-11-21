
namespace SciMaterials.Contracts.Identity.API.Responses.Roles;

public class ClientCreateRoleResponse : Result.Result
{
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}