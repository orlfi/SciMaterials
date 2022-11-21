
using SciMaterials.Contracts.Identity.API.Responses.DTO;

namespace SciMaterials.Contracts.Identity.API.Responses.Roles;

public class ClientGetAllRolesResponse : Result.Result
{
    public string? Message { get; set; }
    public List<AuthRoles>? Roles { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}