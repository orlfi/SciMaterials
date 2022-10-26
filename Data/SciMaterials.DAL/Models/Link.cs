using System.ComponentModel.DataAnnotations;

using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models
{
    public class Link : BaseModel
    {
        public string SourceAddress { get; set; } = string.Empty;
        public string? Hash { get; set; }
        public Guid AuthorId { get; set; }
        public string? Description { get; set; }

        public Author Author { get; set; } = null!;
    }
}
