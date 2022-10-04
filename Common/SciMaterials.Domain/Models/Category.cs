using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class Category : NamedModel
{
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<File> Files { get; set; }

    public Category()
    {
        Files = new HashSet<File>();
    }
}
