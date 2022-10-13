namespace SciMaterials.Contracts.API.DTO.Authors;

public class AddAuthorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}