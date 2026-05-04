using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer.DTOs;
using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationsController : ControllerBase
{
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

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> GetById(string id, CancellationToken ct)
    {
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
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApplicationDto>> Create(
        [FromBody] CreateApplicationDto dto,
        CancellationToken ct)
    {
        // [ApiController] already validates ModelState automatically

        var createdApplication = _applicationService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdApplication.Id },
            createdApplication);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> Update(
        string id,
        [FromBody] UpdateApplicationDto dto,
        CancellationToken ct)
    {
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
    }

}
