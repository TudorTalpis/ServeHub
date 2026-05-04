using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IProviderAction _applicationService = new BusinessLogic().ProviderAction();

    public ApplicationsController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_applicationService.GetAllApplicationAction());

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> GetById(string id, CancellationToken ct)
    {
        var a = _applicationService.GetByIdApplicationAction(id);
        if (a == null) return NotFound(new { message = $"Application {id} not found" });
        return Ok(a);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApplicationDto>> Create(
        [FromBody] CreateApplicationDto dto,
        CancellationToken ct)
    {
        var a = _applicationService.CreateApplicationAction(dto);
        return CreatedAtAction(nameof(GetById), new { id = a.Id }, a);
    }

    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> Update(
        string id,
        [FromBody] UpdateApplicationDto dto,
        CancellationToken ct)
    {
        var a = _applicationService.UpdateApplicationAction(id, dto);
        if (a == null) return NotFound(new { message = $"Application {id} not found" });
        return Ok(a);
    }

}


