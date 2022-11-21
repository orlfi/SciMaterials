namespace SciMaterials.Contracts.API.DTO.Categories;

public class CategoryWithResourcesTreeNode : CategoryTreeNode
{
    public IEnumerable<CategoryTreeResource> Resources { get; set; } = new List<CategoryTreeResource>();
}