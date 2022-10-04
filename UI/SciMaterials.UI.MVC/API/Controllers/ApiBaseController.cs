using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SciMaterials.UI.MVC.API.Controllers
{
    [ApiController]
    public abstract class ApiBaseController<T> : ControllerBase
    {
        private ILogger<T> _loggerInstance;

        protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
    }
}