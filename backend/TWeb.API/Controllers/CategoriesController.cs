using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer.DTOs;
using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoryService categoryService,
        ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken ct)
    {
        var categories = _categoryService.GetAll();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(string id, CancellationToken ct)
    {
        var category = _categoryService.GetById(id);

        if (category == null)
        {
            _logger.LogWarning("Category with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Category not found",
                Detail = $"Category with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryDto dto,
        CancellationToken ct)
    {
        var createdCategory = _categoryService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdCategory.Id },
            createdCategory);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> Update(
        string id,
        [FromBody] UpdateCategoryDto dto,
        CancellationToken ct)
    {
        var updatedCategory = _categoryService.Update(id, dto);

        if (updatedCategory == null)
        {
            _logger.LogWarning("Update failed. Category with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Category not found",
                Detail = $"Category with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(updatedCategory);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted = _categoryService.Delete(id);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed. Category with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Category not found",
                Detail = $"Category with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }
}
