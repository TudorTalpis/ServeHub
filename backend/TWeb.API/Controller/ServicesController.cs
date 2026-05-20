using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.BusinessLayer.Interfaces;
using TWeb.Domain.Models;

namespace TWeb.API.Controller;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceAction _serviceService = new BusinessLogic().ServiceAction();

    public ServicesController() { }

    [HttpGet]
    public IActionResult GetAll() => Ok(_serviceService.GetAllServiceAction());

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var s = _serviceService.GetByIdServiceAction(id);
        if (s == null) return NotFound(new { message = $"Service {id} not found" });
        return Ok(s);
    }

    [HttpGet("provider/{providerId}")]
    public IActionResult GetByProviderId(string providerId) =>
        Ok(_serviceService.GetByProviderIdServiceAction(providerId));

    [Authorize(Roles = "PROVIDER,ADMIN")]
    [HttpPost]
    public IActionResult Create([FromBody] CreateServiceDto dto)
    {
        var s = _serviceService.CreateServiceAction(dto);
        return CreatedAtAction(nameof(GetById), new { id = s.Id }, s);
    }

    [Authorize(Roles = "PROVIDER,ADMIN")]
    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] UpdateServiceDto dto)
    {
        var s = _serviceService.UpdateServiceAction(id, dto);
        if (s == null) return NotFound(new { message = $"Service {id} not found" });
        return Ok(s);
    }

    [Authorize(Roles = "PROVIDER,ADMIN")]
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        if (!_serviceService.DeleteServiceAction(id))
            return NotFound(new { message = $"Service {id} not found" });
        return NoContent();
    }
}
