using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class FileGroup : Resource
{

    public ICollection<File> Files { get; set; }

    public FileGroup() : base()
    {
        Files = new HashSet<File>();
    }
}
