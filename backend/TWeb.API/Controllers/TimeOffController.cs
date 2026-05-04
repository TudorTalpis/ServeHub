using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class TimeOffController : ControllerBase
{
    private readonly IProviderAction _timeOffService = new BusinessLogic().ProviderAction();

    public TimeOffController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_timeOffService.GetAllTimeOffAction());

    [HttpGet("provider/{providerId}")]
    public IActionResult GetByProviderId(string providerId) =>
        Ok(_timeOffService.GetByProviderIdTimeOffAction(providerId));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<TimeOffDto>> Create(
        [FromBody] CreateTimeOffDto dto,
        CancellationToken ct)
    {
        var t = _timeOffService.CreateTimeOffAction(dto);
        return CreatedAtAction(nameof(GetAll), t);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        if (!_timeOffService.DeleteTimeOffAction(id))
            return NotFound(new { message = $"TimeOff {id} not found" });
        return NoContent();
    }
}
