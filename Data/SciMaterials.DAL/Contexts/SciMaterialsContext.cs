using Microsoft.EntityFrameworkCore;
using SciMaterials.DAL.Models;

namespace SciMaterials.DAL.Contexts
{
    public class SciMaterialsContext : DbContext, ISciMaterialsContext
    {
        public SciMaterialsContext(DbContextOptions<SciMaterialsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<ContentType> ContentTypes { get; set; } = null!;
        public virtual DbSet<Models.File> Files { get; set; } = null!;
        public virtual DbSet<FileGroup> FileGroups { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<Author> Authors { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
