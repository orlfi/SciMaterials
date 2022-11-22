namespace SciMaterials.Contracts.API.Settings;

public class ApiSettings
{
    public const string SectionName = "ApiSettings";
    public string BasePath { get; set; } = string.Empty;
    public long MaxFileSize { get; set; }
    public string Separator { get; set; } = ",";
}