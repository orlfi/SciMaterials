using Microsoft.AspNetCore.Components.Forms;

namespace SciMaterials.UI.BWASM.Models;

public class FileUploadData
{
    public FileUploadData(IBrowserFile file)
    {
        File = file;
        FileName = file.Name;
    }

    public IBrowserFile File { get; }
    public string FileName { get; set; }
    public string? Category { get; set; }
}