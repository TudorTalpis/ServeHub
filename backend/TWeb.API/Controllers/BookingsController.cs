using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer.DTOs;
using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingService bookingService,
        ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll(CancellationToken ct)
    {
        var bookings = _bookingService.GetAll();
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> GetById(string id, CancellationToken ct)
    {
        var booking = _bookingService.GetById(id);

        if (booking == null)
        {
            _logger.LogWarning("Booking with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Booking not found",
                Detail = $"Booking with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(booking);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByUserId(
        string userId,
        CancellationToken ct)
    {
        var bookings = _bookingService.GetByUserId(userId);
        return Ok(bookings);
    }

    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByProviderId(
        string providerId,
        CancellationToken ct)
    {
        var bookings = _bookingService.GetByProviderId(providerId);
        return Ok(bookings);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingDto>> Create(
        [FromBody] CreateBookingDto dto,
        CancellationToken ct)
    {
        var booking = _bookingService.Create(dto);

        if (booking == null)
        {
            _logger.LogWarning("Booking conflict for user {UserId} and provider {ProviderId}", dto.UserId, dto.ProviderId);

            return Conflict(new ProblemDetails
            {
                Title = "Booking conflict",
                Detail = "The booking conflicts with an existing one or the provider is unavailable",
                Status = StatusCodes.Status409Conflict
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingDto>> Update(
        string id,
        [FromBody] UpdateBookingDto dto,
        CancellationToken ct)
    {
        var updatedBooking = _bookingService.Update(id, dto);

        if (updatedBooking == null)
        {
            _logger.LogWarning("Update failed. Booking with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Booking not found",
                Detail = $"Booking with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(updatedBooking);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var deleted = _bookingService.Delete(id);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed. Booking with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Booking not found",
                Detail = $"Booking with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }
}