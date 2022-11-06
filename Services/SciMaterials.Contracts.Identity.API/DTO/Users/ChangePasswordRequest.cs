namespace SciMaterials.Contracts.Identity.API.DTO.Users;

public class ChangePasswordRequest
{
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}