using SciMaterials.Contracts.API.Settings;

namespace SciMaterials.Services.Configuration;

public class ApiSettings : IApiSettings
{
    public const string SectionName = "ApiSettings";
    public string BasePath { get; set; } = string.Empty;
    public long MaxFileSize { get; set; }
    public bool OverwriteFile { get; set; }
}