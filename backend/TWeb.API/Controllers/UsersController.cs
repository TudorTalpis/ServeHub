using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer.DTOs;
using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken ct)
    {
        var users =  _userService.GetAll();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(string id, CancellationToken ct)
    {
        var user = _userService.GetById(id);

        if (user == null)
        {
            _logger.LogWarning("User with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = $"User with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(user);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Update(
        string id,
        [FromBody] UpdateUserDto dto,
        CancellationToken ct)
    {
        var updatedUser = _userService.Update(id, dto);

        if (updatedUser == null)
        {
            _logger.LogWarning("Update failed. User {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = $"User with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(updatedUser);
    }
}
