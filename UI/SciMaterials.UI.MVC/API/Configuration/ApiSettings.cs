using SciMaterials.UI.MVC.API.Configuration.Interfaces;

namespace SciMaterials.UI.MVC.API.Configuration;

public class ApiSettings : IApiSettings
{
    public const string SectionName = "ApiSettings";
    public string BasePath { get; set; } = string.Empty;
    public long MaxFileSize { get; set; }
    public bool OverwriteFile { get; set; }
}