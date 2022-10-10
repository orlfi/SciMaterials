using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Settings;

namespace SciMaterials.MVC.Controllers;

public class FilesController : Controller
{
    private readonly ILogger<FilesController> _logger;

    private static string? _message;
    private readonly IApiSettings _apiSettings;

    public FilesController(ILogger<FilesController> logger, IApiSettings apiSettings)
    {
        _logger = logger;

        _apiSettings = apiSettings;
        if (string.IsNullOrEmpty(apiSettings.BasePath))
            throw new ArgumentNullException("Path");
    }

    public IActionResult Index()
    {
        ViewBag.Message = _message;
        return View();
    }
}
