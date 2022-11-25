namespace SciMaterials.DAL.Models.Base;

public abstract class NamedModel : BaseModel
{
    public string Name { get; set; } = string.Empty;
}