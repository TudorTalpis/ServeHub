using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.BusinessLayer.Interfaces;
using TWeb.Domain.Models;

namespace TWeb.API.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TimeOffController : ControllerBase
{
    private readonly IProviderAction _timeOffService = new BusinessLogic().ProviderAction();

    public TimeOffController() { }

    [HttpGet]
    public IActionResult GetAll() => Ok(_timeOffService.GetAllTimeOffAction());

    [HttpGet("provider/{providerId}")]
    public IActionResult GetByProviderId(string providerId) =>
        Ok(_timeOffService.GetByProviderIdTimeOffAction(providerId));

    [Authorize(Roles = "PROVIDER,ADMIN")]
    [HttpPost]
    public IActionResult Create([FromBody] CreateTimeOffDto dto)
    {
        var t = _timeOffService.CreateTimeOffAction(dto);
        return CreatedAtAction(nameof(GetAll), t);
    }

    [Authorize(Roles = "PROVIDER,ADMIN")]
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        if (!_timeOffService.DeleteTimeOffAction(id))
            return NotFound(new { message = $"TimeOff {id} not found" });
        return NoContent();
    }
}
