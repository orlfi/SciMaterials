using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.Contracts.WebAPI.LinkSearch;
using SciMaterials.DAL.Models;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with authors. </summary>
[ApiController]
[Route(WebApiRoute.Links)]
public class LinksController : ApiBaseController<LinksController>
{
    private readonly ILinkShortCut<Link> _linkShortCut;
    private readonly ILogger<LinksController> _logger;

    public LinksController(ILinkShortCut<Link> linkShortCut, ILogger<LinksController> logger)
    {
        _linkShortCut = linkShortCut;
        _logger = logger;
    }

    [HttpGet("{link}")]
    public async Task<IActionResult> GetByIdAsync(string link)
    {
        var result = await _linkShortCut.AddAsync(link);
        return Redirect(result);
    }
}