using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Tags;
using SciMaterials.Contracts.API.Services.Tags;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with tags. </summary>
[ApiController]
[Route(WebApiRoute.Tags)]
public class TagsController : ApiBaseController<TagsController>
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    /// <summary> Get All Tags. </summary>
    /// <returns> Status 200 OK. </returns>
    [HttpGet]
    [ProducesDefaultResponseType(typeof(Result<IEnumerable<GetTagResponse>>))]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _tagService.GetAllAsync();
        return Ok(result);
    }

    /// <summary> Get Tag by Id. </summary>
    /// <param name="id"> Tag Id. </param>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType(typeof(Result<GetTagResponse>))]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _tagService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary> Add a Tag. </summary>
    /// <param name="request"> Add Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Add")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> AddAsync([FromBody] AddTagRequest request)
    {
        var result = await _tagService.AddAsync(request);
        return Ok(result);
    }

    /// <summary> Edit a Tag. </summary>
    /// <param name="request"> Edit Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPut("Edit")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> EditAsync([FromBody] EditTagRequest request)
    {
        var result = await _tagService.EditAsync(request);
        return Ok(result);
    }

    /// <summary> Delete a Tag. </summary>
    /// <param name="id"> Tag Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _tagService.DeleteAsync(id);
        return Ok(result);
    }
}