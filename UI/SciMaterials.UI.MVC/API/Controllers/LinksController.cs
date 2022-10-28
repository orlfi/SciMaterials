using Microsoft.AspNetCore.Mvc;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.API.Services.Tags;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with authors. </summary>
[ApiController]
[Route(WebApiRoute.Links)]
public class LinksController : ApiBaseController<LinksController>
{
    private readonly ILinkShortCut _linkShortCut;

    public LinksController(ILinkShortCut linkShortCut)
    {
        _linkShortCut = linkShortCut;
    }

    [HttpGet("{link}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string link)
    {
        var targetLink = await _linkShortCut.FindByUrlAsync(link);

        return Redirect(targetLink);
    }

}