using BazaR.Backend.Api.Contracts.Reviews;
using BazaR.Backend.Api.Contracts.Reviews.SellerReviews;
using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.CreateSellerReview;
using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.DeleteSellerReviewByUser;
using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.EditSellerReview;
using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.VoteSellerReview;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/seller-reviews")]
[Authorize(Roles = "Customer")]
public sealed class CustomerSellerReviewsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUser _currentUser;

    public CustomerSellerReviewsController(ISender sender, ICurrentUser currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSellerReviewRequest request,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new CreateSellerReviewCommand(
                request.SellerId,
                _currentUser.UserId,
                request.Rating,
                request.Advantages,
                request.Disadvantages,
                request.Body),
            ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(new { reviewId = result.Value });
    }

    [HttpPut("{reviewId:guid}")]
    public async Task<IActionResult> Edit(
        Guid reviewId,
        [FromBody] EditSellerReviewRequest request,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new EditSellerReviewCommand(
                reviewId,
                _currentUser.UserId,
                request.Rating,
                request.Advantages,
                request.Disadvantages,
                request.Body),
            ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> Delete(
        Guid reviewId,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new DeleteSellerReviewByUserCommand(reviewId, _currentUser.UserId),
            ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("{reviewId:guid}/vote")]
    public async Task<IActionResult> Vote(
        Guid reviewId,
        [FromBody] VoteSellerReviewRequest request,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new VoteSellerReviewCommand(
                reviewId,
                _currentUser.UserId,
                request.IsHelpful),
            ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    private IActionResult ProblemFromError(Error error)
    {
        var status = error.Code switch
        {
            "Review.NotFound" => StatusCodes.Status404NotFound,
            "SellerReview.Duplicate" => StatusCodes.Status409Conflict,
            "SellerReview.Forbidden" => StatusCodes.Status403Forbidden,
            "Review.Vote.OwnReview" => StatusCodes.Status400BadRequest,
            "SellerReview.Edit.Empty" => StatusCodes.Status400BadRequest,
            "Review.Content.Required" => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}