namespace SciMaterials.Contracts.API.DTO.Categories;

public record CategoryTree(Guid? Id, string Name, IEnumerable<CategoryTreeInfo> SubCategories, IEnumerable<CategoryTreeResource> Resources)
{
    public IEnumerable<CategoryTreeInfo> SubCategories { get; init; } = SubCategories ?? Enumerable.Empty<CategoryTreeInfo>();
    public IEnumerable<CategoryTreeResource> Resources { get; init; } = Resources ?? Enumerable.Empty<CategoryTreeResource>();
}
