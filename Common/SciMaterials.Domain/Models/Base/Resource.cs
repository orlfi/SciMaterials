namespace SciMaterials.Domain.Models.Base;
public class Resource : NamedModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public User Owner { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}