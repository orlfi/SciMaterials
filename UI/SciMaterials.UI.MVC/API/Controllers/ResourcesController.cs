using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.Result;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.Contracts.API.DTO.Resources;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with resources. </summary>
[ApiController]
[Route(WebApiRoute.Resources)]
public class ResourcesController : ApiBaseController<ResourcesController>
{
    private readonly IResourceService _resourceService;

    public ResourcesController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    /// <summary> Get All Resource. </summary>
    /// <returns> Status 200 OK. </returns>
    [HttpGet]
    [ProducesDefaultResponseType(typeof(Result<IEnumerable<GetResourceResponse>>))]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _resourceService.GetAllAsync();
        return Ok(result);
    }

    /// <summary> Get Resource by Id. </summary>
    /// <param name="id"> Resource Id. </param>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType(typeof(Result<GetResourceResponse>))]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _resourceService.GetByIdAsync(id);
        return Ok(result);
    }
}