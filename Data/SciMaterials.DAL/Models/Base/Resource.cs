namespace SciMaterials.DAL.Models.Base;

public class Resource : NamedModel
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Author Author { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<Rating> Ratings { get; set; }

    public Resource()
    {
        Comments = new HashSet<Comment>();
        Tags = new HashSet<Tag>();
        Categories = new HashSet<Category>();
        Ratings = new HashSet<Rating>();
    }
}