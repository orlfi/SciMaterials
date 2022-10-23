namespace SciMaterials.UI.BWASM.Models;

public class UploadFileDetailsForm
{
    public static readonly UploadFileDetailsForm Empty = new UploadFileDetailsForm();

    public Guid Id { get; init; }
    public string FileName { get; set; } = null!;
    public string? Category { get; set; }
    public long Size { get; init; }
}