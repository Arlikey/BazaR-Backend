using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.CreateProductReview;

public sealed class CreateProductReviewCommandHandler
    : IRequestHandler<CreateProductReviewCommand, Result<Guid>>
{
    private readonly IProductReviewRepository _reviews;
    private readonly ProductRatingSummaryUpdater _summaryUpdater;
    private readonly IUnitOfWork _uow;

    public CreateProductReviewCommandHandler(
        IProductReviewRepository reviews,
        ProductRatingSummaryUpdater summaryUpdater,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _summaryUpdater = summaryUpdater;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateProductReviewCommand request, CancellationToken ct)
    {
        var productId = new ProductId(request.ProductId);
        var authorUserId = new UserId(request.AuthorUserId);

        var existing = await _reviews.GetByProductAndAuthorAsync(productId, authorUserId, ct);
        if (existing is not null)
            return Result<Guid>.Failure(ReviewErrors.DuplicateProductReview);

        var created = ProductReview.Create(
            productId,
            authorUserId,
            request.Rating,
            request.Title,
            request.Body);

        if (created.IsFailure)
            return Result<Guid>.Failure(created.Error);

        var review = created.Value!;
        await _reviews.AddAsync(review, ct);

        var updateSummary = await _summaryUpdater.UpdateAsync(productId, ct);
        if (updateSummary.IsFailure)
            return Result<Guid>.Failure(updateSummary.Error);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(review.Id.Value);
    }
}