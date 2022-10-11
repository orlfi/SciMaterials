namespace SciMaterials.Contracts.API.DTO.Files;

public class AddUrlRequest
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string Tags { get; set; }
    public string Categories { get; set; }
}