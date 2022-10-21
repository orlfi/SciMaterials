namespace SciMaterials.Contracts.API.DTO.AuthUsers;

public class AuthUserRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}