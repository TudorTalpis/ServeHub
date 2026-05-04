using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : ControllerBase
{
<<<<<<< Ion
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
=======
    private readonly IServiceAction _categoryService = new BusinessLogic().ServiceAction();

    public CategoriesController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_categoryService.GetAllCategoryAction());
>>>>>>> main

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(string id, CancellationToken ct)
    {
<<<<<<< Ion
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
=======
        var cat = _categoryService.GetByIdCategoryAction(id);
        if (cat == null) return NotFound(new { message = $"Category {id} not found" });
        return Ok(cat);
>>>>>>> main
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryDto dto,
        CancellationToken ct)
    {
<<<<<<< Ion
        var createdCategory = _categoryService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdCategory.Id },
            createdCategory);
=======
        var cat = _categoryService.CreateCategoryAction(dto);
        return CreatedAtAction(nameof(GetById), new { id = cat.Id }, cat);
>>>>>>> main
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> Update(
        string id,
        [FromBody] UpdateCategoryDto dto,
        CancellationToken ct)
    {
<<<<<<< Ion
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
=======
        var cat = _categoryService.UpdateCategoryAction(id, dto);
        if (cat == null) return NotFound(new { message = $"Category {id} not found" });
        return Ok(cat);
>>>>>>> main
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
<<<<<<< Ion
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

=======
        if (!_categoryService.DeleteCategoryAction(id))
            return NotFound(new { message = $"Category {id} not found" });
>>>>>>> main
        return NoContent();
    }
}


