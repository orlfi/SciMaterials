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

    /// <summary> Add/Edit a Category. </summary>
    /// <param name="request"> Add/edit Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] AddEditCategoryRequest request)
    {
        return Ok(await _сategoryService.AddEditAsync(request));
    }

    /// <summary> Delete a Category. </summary>
    /// <param name="id"> Category Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok(await _сategoryService.DeleteAsync(id));
    }
}