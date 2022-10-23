using Microsoft.AspNetCore.Components.Forms;

namespace SciMaterials.UI.BWASM.Models;

public class FileUploadData
{
    public Guid Id { get; init; }
    public IBrowserFile File { get; init; } = null!;
    public string FileName { get; init; } = null!;

    public CancellationToken CancellationToken { get; init; }
}