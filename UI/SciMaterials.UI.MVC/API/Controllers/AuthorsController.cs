using Microsoft.AspNetCore.Mvc;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.API.DTO.Authors;
using SciMaterials.Contracts.API.Services.Authors;

namespace SciMaterials.UI.MVC.API.Controllers;

/// <summary> Service for working with categories. </summary>
[ApiController]
[Route(WebApiRoute.Authors)]
public class AuthorsController : ApiBaseController<CategoriesController>
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    /// <summary> Get All Categories. </summary>
    /// <returns> Status 200 OK. </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var сategories = await _authorService.GetAllAsync();
        return Ok(сategories);
    }

    /// <summary> Get Author by Id. </summary>
    /// <param name="id"> Author Id. </param>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var products = await _authorService.GetByIdAsync(id);
        return Ok(products);
    }

    /// <summary> Add a Author. </summary>
    /// <param name="request"> Add Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Add")]
    public async Task<IActionResult> AddAsync([FromBody] AddAuthorRequest request)
    {
        return Ok(await _authorService.AddAsync(request));
    }

    /// <summary> Edit a Author. </summary>
    /// <param name="request"> Edit Request DTO. </param>
    /// <returns> Status 200 OK. </returns>
    [HttpPost("Edit")]
    public async Task<IActionResult> EditAsync([FromBody] EditAuthorRequest request)
    {
        return Ok(await _authorService.EditAsync(request));
    }

    /// <summary> Delete a Author. </summary>
    /// <param name="id"> Author Id. </param>
    /// <returns> Status 200 OK response. </returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        return Ok(await _authorService.DeleteAsync(id));
    }
}