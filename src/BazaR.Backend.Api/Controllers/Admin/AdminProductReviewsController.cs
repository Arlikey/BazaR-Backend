using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.ApproveProductReview;
using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.DeleteProductReviewByAdmin;
using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.RejectProductReview;

using BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetPendingProductReviews;
using BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviewsForModeration;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/product-reviews")]
[Authorize(Policy = "Admin")]
public sealed class AdminProductReviewsController : ControllerBase
{
    private readonly ISender _sender;

    public AdminProductReviewsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetPendingProductReviewsQuery(page, pageSize),
            ct);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetForModeration(
        [FromQuery] string? status,
        [FromQuery] Guid? productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetProductReviewsForModerationQuery(status, productId, page, pageSize),
            ct);

        return Ok(result);
    }

    [HttpPost("{reviewId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid reviewId, CancellationToken ct = default)
    {
        var result = await _sender.Send(new ApproveProductReviewCommand(reviewId), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("{reviewId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid reviewId, CancellationToken ct = default)
    {
        var result = await _sender.Send(new RejectProductReviewCommand(reviewId), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> Delete(Guid reviewId, CancellationToken ct = default)
    {
        var result = await _sender.Send(new DeleteProductReviewByAdminCommand(reviewId), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    private IActionResult ProblemFromError(Error error)
    {
        var status = error.Code switch
        {
            "Review.NotFound" => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}