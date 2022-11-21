namespace SciMaterials.Contracts.API.DTO.Categories;

public class CategoryTreeNode
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = "root";
    public IEnumerable<CategoryTreeNode> Children { get; set; } = new List<CategoryTreeNode>();
}