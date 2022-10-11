using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

public class Author : NamedModel
{
    public string Email { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;

    public User? User { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<File> Files { get; set; }
    public ICollection<Rating> Ratings { get; set; }

    public Author()
    {
        Comments = new HashSet<Comment>();
        Files = new HashSet<File>();
        Ratings = new HashSet<Rating>();
    }
}
