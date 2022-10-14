using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class Tag : NamedModel
{
    public ICollection<File> Files { get; set; } = new HashSet<File>();

    public ICollection<FileGroup> FileGroups { get; set; } = new HashSet<FileGroup>();
}
