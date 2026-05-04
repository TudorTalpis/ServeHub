using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly IProviderProfileService _providerService;
    private readonly ILogger<ProvidersController> _logger;

    public ProvidersController(
        IProviderProfileService providerService,
        ILogger<ProvidersController> logger)
    {
        _providerService = providerService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProviderProfileDto>>> GetAll(CancellationToken ct)
    {
        var providers = _providerService.GetAll();
        return Ok(providers);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProviderProfileDto>> GetById(string id, CancellationToken ct)
    {
        var provider =  _providerService.GetById(id);

        if (provider == null)
        {
            _logger.LogWarning("Provider with id {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Provider not found",
                Detail = $"Provider with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(provider);
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProviderProfileDto>> GetBySlug(string slug, CancellationToken ct)
    {
        var provider =  _providerService.GetBySlug(slug);

        if (provider == null)
        {
            _logger.LogWarning("Provider with slug {Slug} not found", slug);

            return NotFound(new ProblemDetails
            {
                Title = "Provider not found",
                Detail = $"Provider with slug '{slug}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(provider);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProviderProfileDto>> GetByUserId(string userId, CancellationToken ct)
    {
        var provider = _providerService.GetByUserId(userId);

        if (provider == null)
        {
            _logger.LogWarning("Provider for user {UserId} not found", userId);

            return NotFound(new ProblemDetails
            {
                Title = "Provider not found",
                Detail = $"Provider for user '{userId}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(provider);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ProviderProfileDto>> Create(
        [FromBody] ProviderProfileDto dto,
        CancellationToken ct)
    {
        var createdProvider = _providerService.Create(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProvider.Id },
            createdProvider);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProviderProfileDto>> Update(
        string id,
        [FromBody] UpdateProviderProfileDto dto,
        CancellationToken ct)
    {
        var updatedProvider =  _providerService.Update(id, dto);

        if (updatedProvider == null)
        {
            _logger.LogWarning("Update failed. Provider {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Provider not found",
                Detail = $"Provider with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(updatedProvider);
    }

[HttpPatch("{id}/featured")]
public IActionResult ToggleFeatured(string id)
{
    if (!_providerService.ToggleFeatured(id))
        return NotFound(new { message = $"Provider {id} not found" });
    return Ok(_providerService.GetById(id));
}

[HttpPatch("{id}/sponsored")]
public IActionResult ToggleSponsored(string id)
{
    if (!_providerService.ToggleSponsored(id))
        return NotFound(new { message = $"Provider {id} not found" });
    return Ok(_providerService.GetById(id));
}

[HttpPatch("{id}/blocked")]
public IActionResult ToggleBlocked(string id)
{
    if (!_providerService.ToggleBlocked(id))
        return NotFound(new { message = $"Provider {id} not found" });
    return Ok(_providerService.GetById(id));
}

    // 🔥 DRY helper method (removes duplication)
    private async Task<ActionResult<ProviderProfileDto>> ToggleAndReturn(
        string id,
        CancellationToken ct,
        Func<string, CancellationToken, Task<ProviderProfileDto?>> toggleFunc)
    {
        var provider = await toggleFunc(id, ct);

        if (provider == null)
        {
            _logger.LogWarning("Toggle failed. Provider {Id} not found", id);

            return NotFound(new ProblemDetails
            {
                Title = "Provider not found",
                Detail = $"Provider with id '{id}' was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(provider);
    }
}


