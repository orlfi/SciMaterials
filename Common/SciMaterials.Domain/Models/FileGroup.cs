using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class FileGroup : Resource
{
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
