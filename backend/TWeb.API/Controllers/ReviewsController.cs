using Microsoft.AspNetCore.Mvc;
using TWeb.BusinessLayer;
using TWeb.Domain.Models;

using TWeb.BusinessLayer.Interfaces;

namespace TWeb.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReviewsController : ControllerBase
{
<<<<<<< Ion
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(
        IReviewService reviewService,
        ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll(CancellationToken ct)
    {
        var reviews =  _reviewService.GetAll();
        return Ok(reviews);
    }
    

    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByProviderId(
        string providerId,
        CancellationToken ct)
    {
        var reviews = _reviewService.GetByProviderId(providerId);
        return Ok(reviews);
    }
=======
    private readonly IServiceAction _reviewService = new BusinessLogic().ServiceAction();

    public ReviewsController()
    {
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_reviewService.GetAllReviewAction());

    [HttpGet("provider/{providerId}")]
    public IActionResult GetByProviderId(string providerId) =>
        Ok(_reviewService.GetByProviderIdReviewAction(providerId));
>>>>>>> main

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ReviewDto>> Create(
        [FromBody] CreateReviewDto dto,
        CancellationToken ct)
    {
<<<<<<< Ion
        var createdReview =  _reviewService.Create(dto);

        return CreatedAtAction(
            nameof(GetAll),
            new { id = createdReview.Id },
            createdReview);
    }
}

=======
        var r = _reviewService.CreateReviewAction(dto);
        return CreatedAtAction(nameof(GetAll), r);
    }
}


>>>>>>> main
