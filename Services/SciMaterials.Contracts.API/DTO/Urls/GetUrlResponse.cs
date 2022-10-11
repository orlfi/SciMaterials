using System.Text.Json.Serialization;

namespace SciMaterials.Contracts.API.DTO.Files;

public class GetFileResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string? Url { get; set; }
    public long Size { get; set; }
    public string Hash { get; set; }

    public Guid? ContentTypeId { get; set; }

    [JsonIgnore]
    public string ContentTypeName { get; set; }
    public Guid? FileGroupId { get; set; }
}