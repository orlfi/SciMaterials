using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Rating : BaseModel
{
    public Guid? FileId { get; set; }
    public Guid? FileGroupId { get; set; }
    public Guid UserId { get; set; }
    public int RatingValue { get; set; }

    public File? File { get; set; } = null!;
    public File? FileGroup { get; set; } = null!;
    public User User { get; set; } = null!;
}