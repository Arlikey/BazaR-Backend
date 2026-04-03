using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.EditSellerReview;

public sealed record EditSellerReviewCommand(
    Guid ReviewId,
    Guid AuthorUserId,
    int? Rating,
    string? Advantages,
    string? Disadvantages,
    string? Body) : IRequest<Result>;