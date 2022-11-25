using System.ComponentModel.DataAnnotations.Schema;

using SciMaterials.DAL.Models.Base;
using SciMaterials.Domain.Enums;

namespace SciMaterials.DAL.Models;

[Table("FileGroups")]
public class FileGroup : Resource
{
    public override ResourceType ResourceType => ResourceType.FileGroup;
    public ICollection<File> Files { get; set; } = new HashSet<File>();
}
