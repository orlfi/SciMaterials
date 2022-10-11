using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class Comment : BaseModel
{
    public Guid? ParentId { get; set; }
    public Guid OwnerId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid ResourceId { get; set; }

    public User Owner { get; set; } = null!;
    public File FileResource { get; set; } = null!;
    public FileGroup FileGroupResource { get; set; } = null!;

    public Comment()
    {
    }
}
