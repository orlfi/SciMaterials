using Microsoft.AspNetCore.Mvc;

namespace SciMaterials.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("check")]
    public IActionResult Check() => Ok();
}