using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TWeb.BusinessLayer;
using TWeb.BusinessLayer.Interfaces;
using TWeb.Domain.Models;

namespace TWeb.API.Controller;

[ApiController]
[Route("api/[controller]")]
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
    public IActionResult Login([FromBody] LoginRequestDto dto)
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
    public IActionResult SignUp([FromBody] SignUpRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required" });

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
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name,  user.Name),
            new Claim("role", user.Role),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
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
