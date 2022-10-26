namespace SciMaterials.UI.BWASM.Models;

public class UploadFileDetailsForm
{
    public static readonly UploadFileDetailsForm Empty = new UploadFileDetailsForm();

    public Guid Id { get; init; }
    public string FileName { get; set; } = null!;
    public long Size { get; init; }

    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}