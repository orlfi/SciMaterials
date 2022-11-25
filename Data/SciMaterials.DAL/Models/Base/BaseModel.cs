namespace SciMaterials.DAL.Models.Base;

public abstract class BaseModel
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
}