
using SciMaterials.DAL.Models;
using File = SciMaterials.DAL.Models.File;

namespace SciMaterials.RepositoryTests.Helpers.ModelsHelpers;

internal static class CategoryHelper
{
    public static IEnumerable<Category> GetMany()
    {
        yield return GetOne();
    }

    public static Category GetOne()
    {
        return new Category()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Description = "Category description",
            Name = "CategoryName",
            FileGroups = new List<FileGroup>()
            {
                new FileGroup()
                {
                    Id = Guid.NewGuid(),
                },
            },
            Files = 
            {
                new File()
                {
                    Id = Guid.NewGuid(),
                },
            },
        };
    }
}