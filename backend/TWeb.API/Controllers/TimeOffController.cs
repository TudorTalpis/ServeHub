using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TimeOffController : ControllerBase
{
<<<<<<< Ion
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
=======
    private readonly IProviderAction _timeOffService = new BusinessLogic().ProviderAction();

    public TimeOffController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_timeOffService.GetAllTimeOffAction());

    [HttpGet("provider/{providerId}")]
    public IActionResult GetByProviderId(string providerId) =>
        Ok(_timeOffService.GetByProviderIdTimeOffAction(providerId));
>>>>>>> main

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<TimeOffDto>> Create(
        [FromBody] CreateTimeOffDto dto,
        CancellationToken ct)
    {
<<<<<<< Ion
        var createdTimeOff =  _timeOffService.Create(dto);

        return CreatedAtAction(
            nameof(GetAll),
            new { id = createdTimeOff.Id },
            createdTimeOff);
=======
        var t = _timeOffService.CreateTimeOffAction(dto);
        return CreatedAtAction(nameof(GetAll), t);
>>>>>>> main
    }

   

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
<<<<<<< Ion
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
=======
        if (!_timeOffService.DeleteTimeOffAction(id))
            return NotFound(new { message = $"TimeOff {id} not found" });
        return NoContent();
    }
}


>>>>>>> main
