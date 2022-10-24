using System.ComponentModel.DataAnnotations;

using SciMaterials.DAL.Models.Base;

namespace SciMaterials.DAL.Models
{
    public class Link : BaseModel
    {
        public string SourceAddress { get; set; } = string.Empty;
        public string? Hash { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime LastAccessedDate { get; set; }
        public int NumberOfRequest { get; set; } = 0;
        [Timestamp] public byte[] TimeStamp { get; set; } = null!;
        public string? Description { get; set; }

        public Author Author { get; set; } = null!;
    }
}
