using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll(CancellationToken ct)
    {
        var notifications = _notificationService.GetAll();
        return Ok(notifications);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByUserId(
        string userId,
        CancellationToken ct)
    {
        var notifications = _notificationService.GetByUserId(userId);
        return Ok(notifications);
    }

   
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<NotificationDto>> Create(
        [FromBody] CreateNotificationDto dto,
        CancellationToken ct)
    {
        var createdNotification = _notificationService.Create(dto);

        return CreatedAtAction(
            nameof(GetAll),
            new { id = createdNotification.Id },
            createdNotification);
    }

    [HttpPatch("{id}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(string id, CancellationToken ct)
    {
        var success = _notificationService.MarkAsRead(id);

        if (!success)
        {
            _logger.LogWarning("MarkAsRead failed. Notification {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Notification not found",
                Detail = $"Notification with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }

    [HttpPatch("user/{userId}/read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllAsRead(string userId, CancellationToken ct)
    {
         _notificationService.MarkAllAsRead(userId);
        return NoContent();
    }
}
