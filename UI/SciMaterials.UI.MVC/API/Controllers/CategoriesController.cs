using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Categories;
using SciMaterials.Contracts.API.Services.Categories;

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
    public async Task<IActionResult> GetAllAsync()
    {
        var сategories = await _сategoryService.GetAllAsync();
        return Ok(сategories);
    }

    /// <summary> Get Category by Id. </summary>
    /// <param name="id"> Category Id. </param>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var products = await _сategoryService.GetByIdAsync(id);
        return Ok(products);
    }

    /// <summary> Add a Category. </summary>
    /// <param name="request"> Add Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Add")]
    public async Task<IActionResult> AddAsync([FromBody] AddCategoryRequest request)
    {
        return Ok(await _сategoryService.AddAsync(request));
    }

    /// <summary> Edit a Category. </summary>
    /// <param name="request"> Edit Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Edit")]
    public async Task<IActionResult> EditAsync([FromBody] EditCategoryRequest request)
    {
        return Ok(await _сategoryService.EditAsync(request));
    }

    /// <summary> Delete a Category. </summary>
    /// <param name="id"> Category Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok(await _сategoryService.DeleteAsync(id));
    }

    [HttpGet("tree/{Id}")]
    //[ProducesResponseType(typeof(CategoryTree), StatusCodes.Status200OK)]
    [ProducesDefaultResponseType(typeof(CategoryTree))]
    public IActionResult GetCategoryTree(Guid Id)
    {
        var result = new CategoryTree(Guid.NewGuid(), "Test", Enumerable.Empty<CategotyTreeInfo>(), Enumerable.Empty<CategoryTreeFile>());
        return Ok(result);
    }

    public record CategotyTreeInfo(Guid Id, string Name);

    public record CategoryTree(Guid Id, string Name, IEnumerable<CategotyTreeInfo> SubCategories, IEnumerable<CategoryTreeFile> Files);

    public record CategoryTreeFile(Guid Id, string Name);
}