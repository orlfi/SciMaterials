namespace SciMaterials.Contracts.API.DTO.AuthRoles;

public class AuthRoleRequest
{
    public string? RoleId { get; set; }
    public string? Email { get; set; }
    public string? RoleName { get; set; }
}