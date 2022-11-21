using System.ComponentModel.DataAnnotations.Schema;

using SciMaterials.DAL.Contracts.Entities.Base;

namespace SciMaterials.DAL.Contracts.Entities;

[Table("FileGroups")]
public class FileGroup : Resource
{
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
