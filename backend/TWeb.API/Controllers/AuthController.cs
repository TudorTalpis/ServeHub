using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer.DTOs;
using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken ct)
    {
        var result =  _userService.Login(dto);

        if (result == null)
        {
            _logger.LogWarning("Failed login attempt for email {Email}", dto.Email);

            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = "Invalid email or password",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(result);
    }

    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> SignUp(
        [FromBody] SignUpRequestDto dto,
        CancellationToken ct)
    {
        // Model validation is automatically handled by [ApiController]
        
        var result = _userService.SignUp(dto);

        if (result == null)
        {
            _logger.LogWarning("Signup failed for email {Email}", dto.Email);

            return BadRequest(new ProblemDetails
            {
                Title = "Signup failed",
                Detail = "User could not be created",
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Created(string.Empty, result);
    }
}
