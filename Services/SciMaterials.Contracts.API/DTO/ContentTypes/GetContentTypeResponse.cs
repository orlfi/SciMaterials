namespace SciMaterials.Contracts.API.DTO.ContentTypes;

public class GetContentTypeResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
}