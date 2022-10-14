using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Tag : NamedModel
{
    public ICollection<File> Files { get; set; } = new HashSet<File>();

    public ICollection<FileGroup> FileGroups { get; set; } = new HashSet<FileGroup>();
}
