using BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviewById;
using BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviews;
using BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviewSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Public;

[ApiController]
[Route("api/sellers")]
[AllowAnonymous]
public sealed class SellerReviewQueriesController : ControllerBase
{
    private readonly ISender _sender;

    public SellerReviewQueriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{sellerId:guid}/reviews")]
    public async Task<IActionResult> GetSellerReviews(
        Guid sellerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetSellerReviewsQuery(sellerId, page, pageSize, sortBy),
            ct);

        return Ok(result);
    }

    [HttpGet("{sellerId:guid}/reviews/summary")]
    public async Task<IActionResult> GetSellerReviewSummary(
        Guid sellerId,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetSellerReviewSummaryQuery(sellerId),
            ct);

        return Ok(result);
    }

    [HttpGet("reviews/{reviewId:guid}")]
    public async Task<IActionResult> GetSellerReviewById(
        Guid reviewId,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetSellerReviewByIdQuery(reviewId), ct);
        if (result is null)
            return NotFound();

        return Ok(result);
    }
}