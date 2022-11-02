using Microsoft.AspNetCore.Mvc;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.ShortenLinks;
using SciMaterials.DAL.Models;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with short links. </summary>
[ApiController]
[Route(WebApiRoute.ShortLinks)]
public class LinksController : ApiBaseController<LinksController>
{
    private readonly ILinkShortCut<Link> _linkShortCut;

    public LinksController(ILinkShortCut<Link> linkShortCut)
        => _linkShortCut = linkShortCut;

    [HttpGet("{shortLink}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string shortLink)
    {
        var targetLink = await _linkShortCut.FindByUrlAsync(shortLink);

        return Redirect(targetLink.SourceAddress);
    }
}