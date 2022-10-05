using Microsoft.EntityFrameworkCore;
using SciMaterials.DAL.Models;

namespace SciMaterials.DAL.Contexts
{
    public class SciMaterialsContext : DbContext
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
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => new { e.FileId, e.UserId });

                entity.HasOne(d => d.File)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ratings_file_id_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ratings_user_id_fk");
            });
        }
    }
}
