using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.BusinessLayer.Interfaces;
using TWeb.Domain.Models;

namespace TWeb.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IProviderAction _applicationService = new BusinessLogic().ProviderAction();

    public ApplicationsController() { }

    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    public IActionResult GetAll() => Ok(_applicationService.GetAllApplicationAction());

    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var a = _applicationService.GetByIdApplicationAction(id);
        if (a == null) return NotFound(new { message = $"Application {id} not found" });
        return Ok(a);
    }

    [Authorize]
    [HttpPost]
    public IActionResult Create([FromBody] CreateApplicationDto dto)
    {
        var a = _applicationService.CreateApplicationAction(dto);
        return CreatedAtAction(nameof(GetById), new { id = a.Id }, a);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] UpdateApplicationDto dto)
    {
        var a = _applicationService.UpdateApplicationAction(id, dto);
        if (a == null) return NotFound(new { message = $"Application {id} not found" });
        return Ok(a);
    }
}
