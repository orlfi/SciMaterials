using SciMaterials.DAL.Contracts.Entities.Base;

namespace SciMaterials.DAL.Contracts.Entities;

public class Tag : NamedModel
{
    public ICollection<Resource> Resources { get; set; } = new HashSet<Resource>();
}
