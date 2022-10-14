namespace SciMaterials.Contracts.API.DTO.ContentTypes;

public class AddContentTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
}