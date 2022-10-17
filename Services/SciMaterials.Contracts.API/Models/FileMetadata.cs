namespace SciMaterials.Contracts.API.Models;

public class FileMetadata
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long Size { get; set; }
    public string ContentTypeName { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public string Categories { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
}