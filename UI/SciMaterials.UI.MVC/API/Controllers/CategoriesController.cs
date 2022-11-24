using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.API.Services.Categories;
using SciMaterials.Contracts.Result;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with categories. </summary>
[ApiController]
[Route(WebApiRoute.Categories)]
public class CategoriesController : ApiBaseController<CategoriesController>
{
    private readonly ICategoryService _сategoryService;

    public CategoriesController(ICategoryService сategoryService)
    {
        _сategoryService = сategoryService;
    }

    /// <summary> Get All Categories. </summary>
    /// <returns> Status 200 OK. </returns>
    [HttpGet]
    [ProducesDefaultResponseType(typeof(Result<IEnumerable<GetCategoryResponse>>))]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _сategoryService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("tree/{id?}")]
    [ProducesDefaultResponseType(typeof(IEnumerable<CategoryTreeNode>))]
    public async Task<IActionResult> GetCategoryTree(Guid? id = null)
    {
        var result = await _сategoryService.GetCategoryWithResourcesTreeAsync(id);
        return Ok(result);
    }

    /// <summary> Get Category by Id. </summary>
    /// <param name="id"> Category Id. </param>
    [HttpGet("{id}")]
    [ProducesDefaultResponseType(typeof(Result<GetCategoryResponse>))]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var result = await _сategoryService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary> Add a Category. </summary>
    /// <param name="request"> Add Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Add")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> AddAsync([FromBody] AddCategoryRequest request)
    {
        var result = await _сategoryService.AddAsync(request);
        return Ok(result);
    }

    /// <summary> Edit a Category. </summary>
    /// <param name="request"> Edit Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPut("Edit")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> EditAsync([FromBody] EditCategoryRequest request)
    {
        var result = await _сategoryService.EditAsync(request);
        return Ok(result);
    }

    /// <summary> Delete a Category. </summary>
    /// <param name="id"> Category Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    [ProducesDefaultResponseType(typeof(Guid))]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _сategoryService.DeleteAsync(id);
        return Ok(result);
    }
}