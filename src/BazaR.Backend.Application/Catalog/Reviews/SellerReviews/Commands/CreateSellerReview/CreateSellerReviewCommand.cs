using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.CreateSellerReview;

public sealed record CreateSellerReviewCommand(
    Guid SellerId,
    Guid AuthorUserId,
    int Rating,
    string Title,
    string Body) : IRequest<Result<Guid>>;