
namespace SciMaterials.Contracts.Identity.API.Responses.Roles;

public class ClientAddRoleToUserResponse : Result.Result
{
    public string? NewToken { get; set; }
    public string? Message { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}