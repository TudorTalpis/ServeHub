using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceAction _serviceService = new BusinessLogic().ServiceAction();

    public ServicesController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_serviceService.GetAllServiceAction());

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> GetById(string id, CancellationToken ct)
    {
        var s = _serviceService.GetByIdServiceAction(id);
        if (s == null) return NotFound(new { message = $"Service {id} not found" });
        return Ok(s);
    }

    [HttpGet("provider/{providerId}")]
    public IActionResult GetByProviderId(string providerId) =>
        Ok(_serviceService.GetByProviderIdServiceAction(providerId));

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ServiceDto>> Create(
        [FromBody] CreateServiceDto dto,
        CancellationToken ct)
    {
        var s = _serviceService.CreateServiceAction(dto);
        return CreatedAtAction(nameof(GetById), new { id = s.Id }, s);
    }

    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> Update(
        string id,
        [FromBody] UpdateServiceDto dto,
        CancellationToken ct)
    {
        var s = _serviceService.UpdateServiceAction(id, dto);
        if (s == null) return NotFound(new { message = $"Service {id} not found" });
        return Ok(s);
    }

    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        if (!_serviceService.DeleteServiceAction(id))
            return NotFound(new { message = $"Service {id} not found" });
        return NoContent();
    }
}
