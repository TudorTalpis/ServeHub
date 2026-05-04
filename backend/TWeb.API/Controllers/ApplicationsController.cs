using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationsController : ControllerBase
{
<<<<<<< Ion
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(
        IApplicationService applicationService,
        ILogger<ApplicationsController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetAll(CancellationToken ct)
    {
        var applications = _applicationService.GetAll();
        return Ok(applications);
    }
=======
    private readonly IProviderAction _applicationService = new BusinessLogic().ProviderAction();

    public ApplicationsController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_applicationService.GetAllApplicationAction());
>>>>>>> main

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> GetById(string id, CancellationToken ct)
    {
<<<<<<< Ion
        var application = _applicationService.GetById(id);

        if (application == null)
        {
            _logger.LogWarning("Application with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Application not found",
                Detail = $"Application with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(application);
=======
        var a = _applicationService.GetByIdApplicationAction(id);
        if (a == null) return NotFound(new { message = $"Application {id} not found" });
        return Ok(a);
>>>>>>> main
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApplicationDto>> Create(
        [FromBody] CreateApplicationDto dto,
        CancellationToken ct)
    {
<<<<<<< Ion
        // [ApiController] already validates ModelState automatically

        var createdApplication = _applicationService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdApplication.Id },
            createdApplication);
=======
        var a = _applicationService.CreateApplicationAction(dto);
        return CreatedAtAction(nameof(GetById), new { id = a.Id }, a);
>>>>>>> main
    }

    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> Update(
        string id,
        [FromBody] UpdateApplicationDto dto,
        CancellationToken ct)
    {
<<<<<<< Ion
        var updatedApplication = _applicationService.Update(id, dto);

        if (updatedApplication == null)
        {
            _logger.LogWarning("Update failed. Application with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Application not found",
                Detail = $"Application with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(updatedApplication);
=======
        var a = _applicationService.UpdateApplicationAction(id, dto);
        if (a == null) return NotFound(new { message = $"Application {id} not found" });
        return Ok(a);
>>>>>>> main
    }

}


