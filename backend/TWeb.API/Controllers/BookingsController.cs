using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingAction _bookingService = new BusinessLogic().BookingAction();

    public BookingsController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_bookingService.GetAllBookingAction());

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(string id, CancellationToken ct)
    {
        var b = _bookingService.GetByIdBookingAction(id);
        if (b == null) return NotFound(new { message = $"Booking {id} not found" });
        return Ok(b);
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetByUserId(string userId) =>
        Ok(_bookingService.GetByUserIdBookingAction(userId));

    [HttpGet("provider/{providerId}")]
    public IActionResult GetByProviderId(string providerId) =>
        Ok(_bookingService.GetByProviderIdBookingAction(providerId));

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Create([FromBody] CreateBookingDto dto)
    {
        var b = _bookingService.CreateBookingAction(dto);
        if (b == null) return Conflict(new { message = "Booking conflict or provider is blocked" });
        return CreatedAtAction(nameof(GetById), new { id = b.Id }, b);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> Update(
        string id,
        [FromBody] UpdateBookingDto dto,
        CancellationToken ct)
    {
        var b = _bookingService.UpdateBookingAction(id, dto);
        if (b == null) return NotFound(new { message = $"Booking {id} not found" });
        return Ok(b);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        if (!_bookingService.DeleteBookingAction(id))
            return NotFound(new { message = $"Booking {id} not found" });
        return NoContent();
    }
}
