namespace SciMaterials.Contracts.API.DTO.Files;

public class EditFileRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortInfo { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public string Categories { get; set; } = string.Empty;
}