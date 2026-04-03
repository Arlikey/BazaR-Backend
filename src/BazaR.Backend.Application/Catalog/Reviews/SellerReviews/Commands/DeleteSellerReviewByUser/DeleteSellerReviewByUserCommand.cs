using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.DeleteSellerReviewByUser;

public sealed record DeleteSellerReviewByUserCommand(
    Guid ReviewId,
    Guid AuthorUserId) : IRequest<Result>;