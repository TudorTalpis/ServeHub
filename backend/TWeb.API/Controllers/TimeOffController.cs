using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer.DTOs;
using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TimeOffController : ControllerBase
{
    private readonly ITimeOffService _timeOffService;
    private readonly ILogger<TimeOffController> _logger;

    public TimeOffController(
        ITimeOffService timeOffService,
        ILogger<TimeOffController> logger)
    {
        _timeOffService = timeOffService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimeOffDto>>> GetAll(CancellationToken ct)
    {
        var timeOffs = _timeOffService.GetAll();
        return Ok(timeOffs);
    }

    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimeOffDto>>> GetByProviderId(
        string providerId,
        CancellationToken ct)
    {
        var timeOffs =  _timeOffService.GetByProviderId(providerId);
        return Ok(timeOffs);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<TimeOffDto>> Create(
        [FromBody] CreateTimeOffDto dto,
        CancellationToken ct)
    {
        var createdTimeOff =  _timeOffService.Create(dto);

        return CreatedAtAction(
            nameof(GetAll),
            new { id = createdTimeOff.Id },
            createdTimeOff);
    }

   

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted =  _timeOffService.Delete(id);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed. TimeOff {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "TimeOff not found",
                Detail = $"TimeOff with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }
}