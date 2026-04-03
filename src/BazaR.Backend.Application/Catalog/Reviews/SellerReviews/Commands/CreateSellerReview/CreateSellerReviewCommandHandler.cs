using BazaR.Backend.Application.Abstractions.Persistence;

using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.SellerReviews;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.CreateSellerReview;

public sealed class CreateSellerReviewCommandHandler
    : IRequestHandler<CreateSellerReviewCommand, Result<Guid>>
{
    private readonly ISellerReviewRepository _reviews;
    private readonly SellerRatingSummaryUpdater _summaryUpdater;
    private readonly IUnitOfWork _uow;

    public CreateSellerReviewCommandHandler(
        ISellerReviewRepository reviews,
        SellerRatingSummaryUpdater summaryUpdater,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _summaryUpdater = summaryUpdater;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateSellerReviewCommand request, CancellationToken ct)
    {
        var sellerId = new SellerId(request.SellerId);
        var authorUserId = new UserId(request.AuthorUserId);

        var existing = await _reviews.GetBySellerAndAuthorAsync(sellerId, authorUserId, ct);
        if (existing is not null)
            return Result<Guid>.Failure(ReviewErrors.DuplicateSellerReview);

        var created = SellerReview.Create(
            sellerId,
            authorUserId,
            request.Rating,
            request.Title,
            request.Body);

        if (created.IsFailure)
            return Result<Guid>.Failure(created.Error);

        var review = created.Value!;
        await _reviews.AddAsync(review, ct);

        var updateSummary = await _summaryUpdater.UpdateAsync(sellerId, ct);
        if (updateSummary.IsFailure)
            return Result<Guid>.Failure(updateSummary.Error);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(review.Id.Value);
    }
}