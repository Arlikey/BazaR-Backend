using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Queries.GetProductReviews;
using BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviewById;

using BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviewSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Public;

[ApiController]
[Route("api/products")]
[AllowAnonymous]
public sealed class ProductReviewQueriesController : ControllerBase
{
    private readonly ISender _sender;

    public ProductReviewQueriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{productId:guid}/reviews")]
    public async Task<IActionResult> GetProductReviews(
        Guid productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetProductReviewsQuery(productId, page, pageSize, sortBy),
            ct);

        return Ok(result);
    }

    [HttpGet("{productId:guid}/reviews/summary")]
    public async Task<IActionResult> GetProductReviewSummary(
        Guid productId,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetProductReviewSummaryQuery(productId),
            ct);

        return Ok(result);
    }

    [HttpGet("reviews/{reviewId:guid}")]
    public async Task<IActionResult> GetProductReviewById(
        Guid reviewId,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetProductReviewByIdQuery(reviewId), ct);
        if (result is null)
            return NotFound();

        return Ok(result);
    }
}