using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryAction _categoryService = new BusinessLogic().CategoryAction();

    public CategoriesController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_categoryService.GetAllCategoryAction());

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(string id, CancellationToken ct)
    {
        var cat = _categoryService.GetByIdCategoryAction(id);
        if (cat == null) return NotFound(new { message = $"Category {id} not found" });
        return Ok(cat);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryDto dto,
        CancellationToken ct)
    {
        var cat = _categoryService.CreateCategoryAction(dto);
        return CreatedAtAction(nameof(GetById), new { id = cat.Id }, cat);
    }

    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> Update(
        string id,
        [FromBody] UpdateCategoryDto dto,
        CancellationToken ct)
    {
        var cat = _categoryService.UpdateCategoryAction(id, dto);
        if (cat == null) return NotFound(new { message = $"Category {id} not found" });
        return Ok(cat);
    }

    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        if (!_categoryService.DeleteCategoryAction(id))
            return NotFound(new { message = $"Category {id} not found" });
        return NoContent();
    }
}


