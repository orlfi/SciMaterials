namespace SciMaterials.Contracts.API.DTO.Passwords;

public class ChangePasswordRequest
{
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}