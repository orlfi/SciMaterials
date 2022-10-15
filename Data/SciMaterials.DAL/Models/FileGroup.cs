using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class FileGroup : Resource
{
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
