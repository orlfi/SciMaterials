using Microsoft.AspNetCore.Components.Forms;

namespace SciMaterials.UI.BWASM.Models;

public class FileUploadData
{
    public Guid Id { get; init; }
    public IBrowserFile File { get; init; } = null!;
    public string FileName { get; init; } = null!;

    public string ContentType { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Category { get; init; } = null!;

    public CancellationToken CancellationToken { get; init; }
}