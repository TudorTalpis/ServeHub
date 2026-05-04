using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewAction _reviewService = new BusinessLogic().ReviewAction();

    public ReviewsController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_reviewService.GetAllReviewAction());

    [HttpGet("provider/{providerId}")]
    public IActionResult GetByProviderId(string providerId) =>
        Ok(_reviewService.GetByProviderIdReviewAction(providerId));

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ReviewDto>> Create(
        [FromBody] CreateReviewDto dto,
        CancellationToken ct)
    {
        var r = _reviewService.CreateReviewAction(dto);
        return CreatedAtAction(nameof(GetAll), r);
    }
}
