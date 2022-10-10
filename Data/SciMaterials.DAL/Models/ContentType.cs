using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class ContentType : NamedModel
{
    public string FileExtension { get; set; } = string.Empty;
    public ICollection<File> Files { get; set; }

    public ContentType()
    {
        Files = new HashSet<File>();
    }
}
