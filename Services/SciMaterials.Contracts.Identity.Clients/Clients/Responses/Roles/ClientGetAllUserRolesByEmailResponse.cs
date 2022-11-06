
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.DTO;

namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientGetAllUserRolesByEmailResponse : Result.Result
{
    public string? Message { get; set; }
    public List<AuthRoles> Roles { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}