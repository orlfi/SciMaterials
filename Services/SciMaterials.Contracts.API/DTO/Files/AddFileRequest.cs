namespace SciMaterials.Contracts.API.DTO.Files;

public class AddFileRequest
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long Size { get; set; }
    public Guid ContentTypeId { get; set; }
    public string? Tags { get; set; }
    public string Categories { get; set; } = string.Empty;
}