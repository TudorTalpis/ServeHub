using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.BusinessLayer.Interfaces;
using TWeb.Domain.Models;

namespace TWeb.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserAction _userService = new BusinessLogic().UserAction();

    public UsersController() { }

    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    public IActionResult GetAll() => Ok(_userService.GetAllUserAction());

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var user = _userService.GetUserByIdAction(id);
        if (user == null) return NotFound(new { message = $"User {id} not found" });
        return Ok(user);
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] UpdateUserDto dto)
    {
        var user = _userService.UpdateUserAction(id, dto);
        if (user == null) return NotFound(new { message = $"User {id} not found" });
        return Ok(user);
    }

    [Authorize]
    [HttpPatch("{id}/password")]
    public IActionResult ChangePassword(string id, [FromBody] ChangePasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            return BadRequest(new { message = "New password must be at least 6 characters" });

        var success = _userService.ChangePasswordAction(id, dto);
        if (!success) return BadRequest(new { message = "Current password is incorrect" });
        return NoContent();
    }
}
