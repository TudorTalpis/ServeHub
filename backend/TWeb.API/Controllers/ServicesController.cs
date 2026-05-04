using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer.DTOs;
using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(
        IServiceService serviceService,
        ILogger<ServicesController> logger)
    {
        _serviceService = serviceService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll(CancellationToken ct)
    {
        var services = _serviceService.GetAll();
        return Ok(services);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> GetById(string id, CancellationToken ct)
    {
        var service =  _serviceService.GetById(id);

        if (service == null)
        {
            _logger.LogWarning("Service with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Service not found",
                Detail = $"Service with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(service);
    }

    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetByProviderId(
        string providerId,
        CancellationToken ct)
    {
        var services =  _serviceService.GetByProviderId(providerId);
        return Ok(services);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ServiceDto>> Create(
        [FromBody] CreateServiceDto dto,
        CancellationToken ct)
    {
        var createdService = _serviceService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdService.Id },
            createdService);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> Update(
        string id,
        [FromBody] UpdateServiceDto dto,
        CancellationToken ct)
    {
        var updatedService = _serviceService.Update(id, dto);

        if (updatedService == null)
        {
            _logger.LogWarning("Update failed. Service {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Service not found",
                Detail = $"Service with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(updatedService);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted =  _serviceService.Delete(id);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed. Service {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Service not found",
                Detail = $"Service with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }
}