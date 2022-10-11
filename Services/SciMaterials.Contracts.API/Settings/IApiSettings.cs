namespace SciMaterials.Contracts.API.Settings;

public interface IApiSettings
{
    string BasePath { get; set; }
    long MaxFileSize { get; set; }
    bool OverwriteFile { get; set; }
}
