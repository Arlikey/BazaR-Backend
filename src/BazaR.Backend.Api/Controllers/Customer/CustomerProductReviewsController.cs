using BazaR.Backend.Api.Contracts.Reviews;
using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.CreateProductReview;
using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.DeleteProductReviewByUser;
using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.EditProductReview;
using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.VoteProductReview;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/product-reviews")]
[Authorize(Roles = "Customer")]
public sealed class CustomerProductReviewsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUser _currentUser;

    public CustomerProductReviewsController(ISender sender, ICurrentUser currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductReviewRequest request,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new CreateProductReviewCommand(
                request.ProductId,
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
        [FromBody] EditProductReviewRequest request,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new EditProductReviewCommand(
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
            new DeleteProductReviewByUserCommand(reviewId, _currentUser.UserId),
            ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("{reviewId:guid}/vote")]
    public async Task<IActionResult> Vote(
        Guid reviewId,
        [FromBody] VoteProductReviewRequest request,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new VoteProductReviewCommand(
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
            "ProductReview.Duplicate" => StatusCodes.Status409Conflict,
            "ProductReview.Forbidden" => StatusCodes.Status403Forbidden,
            "Review.Vote.OwnReview" => StatusCodes.Status400BadRequest,
            "ProductReview.Edit.Empty" => StatusCodes.Status400BadRequest,
            "Review.Content.Required" => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}