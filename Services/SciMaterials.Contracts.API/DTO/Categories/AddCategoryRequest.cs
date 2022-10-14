namespace SciMaterials.Contracts.API.DTO.Categories;

public class AddCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? Description { get; set; }
}