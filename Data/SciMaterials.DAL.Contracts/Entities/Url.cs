using System.ComponentModel.DataAnnotations.Schema;

using SciMaterials.DAL.Contracts.Entities.Base;

namespace SciMaterials.DAL.Contracts.Entities;

[Table("Urls")]
public class Url : Resource
{
    public string? Link { get; set; }
}
