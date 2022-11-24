using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Urls;
using SciMaterials.Contracts.API.Services.Urls;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with urls. </summary>
[ApiController]
[Route(WebApiRoute.Urls)]
public class UrlsController : ApiBaseController<UrlsController>
{
    private readonly IUrlService _urlService;

    public UrlsController(IUrlService urlService)
    {
        _urlService = urlService;
    }

    /// <summary> Get All Urls. </summary>
    /// <returns> Status 200 OK. </returns>
    [HttpGet]
    [ProducesDefaultResponseType(typeof(Result<IEnumerable<GetUrlResponse>>))]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _urlService.GetAllAsync();
        return Ok(result);
    }

    /// <summary> Get Url by Id. </summary>
    /// <param name="id"> Url Id. </param>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType(typeof(Result<GetUrlResponse>))]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _urlService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary> Add a Url. </summary>
    /// <param name="request"> Add Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Add")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> AddAsync([FromBody] AddUrlRequest request)
    {
        var result = await _urlService.AddAsync(request);
        return Ok(result);
    }

    /// <summary> Edit a Url. </summary>
    /// <param name="request"> Edit Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPut("Edit")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> EditAsync([FromBody] EditUrlRequest request)
    {
        var result = await _urlService.EditAsync(request);
        return Ok(result);
    }

    /// <summary> Delete a Url. </summary>
    /// <param name="id"> Url Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _urlService.DeleteAsync(id);
        return Ok(result);
    }
}