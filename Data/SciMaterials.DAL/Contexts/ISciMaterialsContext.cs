using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using SciMaterials.DAL.Contracts.Entities;

using File = SciMaterials.DAL.Contracts.Entities.File;

namespace SciMaterials.DAL.Contexts;

public interface ISciMaterialsContext
{
    DbSet<Category> Categories { get; set; }
    DbSet<Comment> Comments { get; set; }
    DbSet<ContentType> ContentTypes { get; set; }
    DbSet<FileGroup> FileGroups { get; set; }
    DbSet<File> Files { get; set; }
    DbSet<Url> Urls { get; set; }
    DbSet<Rating> Ratings { get; set; }
    DbSet<Tag> Tags { get; set; }
    DbSet<Author> Authors { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<T> Set<T>() where T : class;
    DbSet<Link> Links { get; set; }
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

}