namespace SciMaterials.UI.BWASM.Models;

public class UploadFileDetailsForm
{
    public static UploadFileDetailsForm Empty { get; } = new UploadFileDetailsForm();

    public Guid Id { get; private init; }
    public string FileName { get; set; } = null!;
    public string? Category { get; set; }
    public long Size { get; private init; }

    public void UpdateSource(FileUploadData? source)
    {
        // source may be already deleted! TODO: why it already not handled on delete?
        if (source is null) return;

        source.FileName = FileName;
        source.Category = Category;
    }

    public static implicit operator UploadFileDetailsForm(FileUploadData data) => new()
    {
        Id = data.Id,
        FileName = data.FileName,
        Category = data.Category,
        Size = data.File.Size
    };
}