using SciMaterials.DAL.Contracts.Entities.Base;

namespace SciMaterials.DAL.Contracts.Entities;

public class ContentType : NamedModel
{
    public string FileExtension { get; set; } = null!;
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
