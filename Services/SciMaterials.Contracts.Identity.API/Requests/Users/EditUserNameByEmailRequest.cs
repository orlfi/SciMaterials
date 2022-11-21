namespace SciMaterials.Contracts.Identity.API.Requests.Users;

public class EditUserNameByEmailRequest
{
    public string? UserEmail { get; set; }
    public string? EditUserNickName { get; set; }
}