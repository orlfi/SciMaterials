namespace SciMaterials.Contracts.Identity.API.DTO.Users;

public class EditUserNameByEmailRequest
{
    public string? UserEmail { get; set; }
    public string? EditUserNickName { get; set; }
}