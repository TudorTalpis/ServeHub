using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TWeb.BusinessLayer;
using TWeb.BusinessLayer.Interfaces;
using TWeb.Domain.Models;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserAction _userService;
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _userService = new BusinessLogic().UserAction();
        _config = config;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken ct)
    {
        var user = _userService.UserLoginAction(dto);
        if (user == null) return Unauthorized(new { message = "Invalid email or password" });
        return Ok(new LoginResponseDto
        {
            Token = GenerateJwtToken(user),
            UserId = user.Id,
            Role = user.Role,
            Name = user.Name,
            Email = user.Email,
            IsDemo = user.Email.EndsWith("@demo.com", StringComparison.OrdinalIgnoreCase)
        });
    }

    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> SignUp(
        [FromBody] SignUpRequestDto dto,
        CancellationToken ct)
    {
        // Model validation is automatically handled by [ApiController]
        var user = _userService.UserSignUpAction(dto);
        return Ok(new LoginResponseDto
        {
            Token = GenerateJwtToken(user),
            UserId = user.Id,
            Role = user.Role,
            Name = user.Name,
            Email = user.Email
        });
    }

    private string GenerateJwtToken(UserDto user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, user.Name)
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(int.Parse(_config["Jwt:ExpiryHours"]!)),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
