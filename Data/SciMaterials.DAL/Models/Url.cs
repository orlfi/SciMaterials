using System.ComponentModel.DataAnnotations.Schema;

using SciMaterials.DAL.Models.Base;
using SciMaterials.Domain.Enums;

namespace SciMaterials.DAL.Models;

[Table("Urls")]
public class Url : Resource
{
    public override ResourceType ResourceType => ResourceType.Url;
    public string? Link { get; set; }
}
