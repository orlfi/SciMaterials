
namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientGetAllRolesResponse : Result.Result
{
    public string? Message { get; set; }
    public List<string>? RoleNames { get; set; }
    public List<string>? RoleIds { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}