using System.ComponentModel.DataAnnotations.Schema;

using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models;

[Table("Files")]
public class File : Resource
{
    public long Size { get; set; }
    public string? Hash { get; set; }
    public Guid? ContentTypeId { get; set; }
    public Guid? FileGroupId { get; set; }
    public int AntivirusScanStatus { get; set; }
    public DateTime? AntivirusScanDate { get; set; }
    string ShortLink { get; set; }

    public ContentType? ContentType { get; set; }
    public FileGroup? FileGroup { get; set; }
}
