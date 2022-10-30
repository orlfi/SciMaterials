
namespace SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;

public class ClientGetRoleByIdResponse : Result.Result
{
    public string? Message { get; set; }
    public string? RoleName { get; set; }
    public string? RoleId { get; set; }
    public int Code { get; set; }
    public bool Succeeded { get; set; }
}