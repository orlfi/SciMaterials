namespace SciMaterials.UI.BWASM.Models;

public class UploadFilesMetadataForm
{
    public static readonly UploadFilesMetadataForm Empty = new UploadFilesMetadataForm();

    public Guid Id { get; init; }
    
    public string ShortInfo { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public CategoryInfo Category { get; set; }
    public AuthorInfo Author { get; set; }

    public record struct CategoryInfo(Guid Id, string Name);
    public record struct AuthorInfo(Guid Id, string FirstName, string Surname);
}