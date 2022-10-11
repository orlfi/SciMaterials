using Microsoft.EntityFrameworkCore;
using SciMaterials.DAL.Models;

namespace SciMaterials.DAL.Contexts
{
    public interface ISciMaterialsContext
    {
        DbSet<Category> Categories { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<ContentType> ContentTypes { get; set; }
        DbSet<FileGroup> FileGroups { get; set; }
        DbSet<Models.File> Files { get; set; }
        DbSet<Rating> Ratings { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Author> Authors { get; set; }
        DbSet<User> Users { get; set; }
    }
}