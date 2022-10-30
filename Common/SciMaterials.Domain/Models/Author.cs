using SciMaterials.Domain.Models.Base;

namespace SciMaterials.Domain.Models;

public class Author : NamedModel
{
    public string Email { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;

    public User? User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public ICollection<File> Files { get; set; } = new HashSet<File>();
    public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}
