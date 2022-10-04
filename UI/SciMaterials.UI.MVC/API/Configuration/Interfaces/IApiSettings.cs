namespace SciMaterials.UI.MVC.API.Configuration.Interfaces;

public interface IApiSettings
{
    string BasePath { get; set; }
    long MaxFileSize { get; set; }
    bool OverwriteFile { get; set; }
}
