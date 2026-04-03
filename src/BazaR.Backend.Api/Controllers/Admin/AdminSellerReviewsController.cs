using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.ApproveSellerReview;
using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.DeleteSellerReviewByAdmin;
using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.RejectSellerReview;

using BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetPendingSellerReviews;
using BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviewsForModeration;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/seller-reviews")]
[Authorize(Policy = "Admin")]
public sealed class AdminSellerReviewsController : ControllerBase
{
    private readonly ISender _sender;

    public AdminSellerReviewsController(ISender sender)
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
            new GetPendingSellerReviewsQuery(page, pageSize),
            ct);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetForModeration(
        [FromQuery] string? status,
        [FromQuery] Guid? sellerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetSellerReviewsForModerationQuery(status, sellerId, page, pageSize),
            ct);

        return Ok(result);
    }

    [HttpPost("{reviewId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid reviewId, CancellationToken ct = default)
    {
        var result = await _sender.Send(new ApproveSellerReviewCommand(reviewId), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("{reviewId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid reviewId, CancellationToken ct = default)
    {
        var result = await _sender.Send(new RejectSellerReviewCommand(reviewId), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> Delete(Guid reviewId, CancellationToken ct = default)
    {
        var result = await _sender.Send(new DeleteSellerReviewByAdminCommand(reviewId), ct);
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