using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SciMaterials.DAL.Contexts;

#nullable disable

namespace SciMaterials.MsSqlServerMigrations.Migrations
{
    [DbContext(typeof(SciMaterialsContext))]
    partial class SciMaterialsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("FileGroupTag", b =>
                {
                    b.Property<Guid>("FileGroupsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TagsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FileGroupsId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("FileGroupTag");
                });

            modelBuilder.Entity("FileTag", b =>
                {
                    b.Property<Guid>("FilesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TagsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FilesId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("FileTag");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ResourceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ResourceId");

                    b.HasIndex(new[] { "OwnerId" }, "IX_Comments_OwnerId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.ContentType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ContentTypes");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.File", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ContentTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("FileGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ContentTypeId");

                    b.HasIndex("FileGroupId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.FileGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("OwnerId");

                    b.ToTable("FileGroups");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Rating", b =>
                {
                    b.Property<Guid>("FileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("FileGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RatingValue")
                        .HasColumnType("int");

                    b.HasKey("FileId", "UserId");

                    b.HasIndex("FileGroupId");

                    b.HasIndex("UserId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FileGroupTag", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.FileGroup", null)
                        .WithMany()
                        .HasForeignKey("FileGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FileTag", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.File", null)
                        .WithMany()
                        .HasForeignKey("FilesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Models.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Comment", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.User", "Owner")
                        .WithMany("Comments")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Models.File", "FileResource")
                        .WithMany("Comments")
                        .HasForeignKey("ResourceId")
                        .IsRequired()
                        .HasConstraintName("comments_files_resourseId_fk");

                    b.HasOne("SciMaterials.DAL.Models.FileGroup", "FileGroupResource")
                        .WithMany("Comments")
                        .HasForeignKey("ResourceId")
                        .IsRequired()
                        .HasConstraintName("comments_file_groups_resourseId_fk");

                    b.Navigation("FileGroupResource");

                    b.Navigation("FileResource");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.File", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Category", "Category")
                        .WithMany("Files")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Models.ContentType", "ContentType")
                        .WithMany("Files")
                        .HasForeignKey("ContentTypeId");

                    b.HasOne("SciMaterials.DAL.Models.FileGroup", "FileGroup")
                        .WithMany("Files")
                        .HasForeignKey("FileGroupId");

                    b.HasOne("SciMaterials.DAL.Models.User", "Owner")
                        .WithMany("Files")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("ContentType");

                    b.Navigation("FileGroup");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.FileGroup", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SciMaterials.DAL.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Rating", b =>
                {
                    b.HasOne("SciMaterials.DAL.Models.FileGroup", null)
                        .WithMany("Ratings")
                        .HasForeignKey("FileGroupId");

                    b.HasOne("SciMaterials.DAL.Models.File", "File")
                        .WithMany("Ratings")
                        .HasForeignKey("FileId")
                        .IsRequired()
                        .HasConstraintName("ratings_file_id_fk");

                    b.HasOne("SciMaterials.DAL.Models.User", "User")
                        .WithMany("Ratings")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("ratings_user_id_fk");

                    b.Navigation("File");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.Category", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.ContentType", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.File", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Ratings");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.FileGroup", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Files");

                    b.Navigation("Ratings");
                });

            modelBuilder.Entity("SciMaterials.DAL.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Files");

                    b.Navigation("Ratings");
                });
#pragma warning restore 612, 618
        }
    }
}