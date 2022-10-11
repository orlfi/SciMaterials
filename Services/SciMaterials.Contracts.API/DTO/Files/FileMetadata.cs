namespace SciMaterials.Contracts.API.DTO.Files;

public class FileMetadata
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long Size { get; set; }
    public string Hash { get; set; }
    public string ContentTypeName { get; set; }
}