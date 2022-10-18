namespace SciMaterials.Contracts.API.DTO.AuthUsers;

public class EditUserRequest
{
    public string? Email { get; set; }
    public AuthUserRequest? EditUserInfo { get; set; }
}