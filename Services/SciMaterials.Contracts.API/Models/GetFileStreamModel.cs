namespace SciMaterials.Contracts.API.Models;

public class GetFileStreamModel
{
    public Stream FileStream { get; set; }
    public string FileName { get; set; }
    public string ContentTypeName{ get; set; }
}
