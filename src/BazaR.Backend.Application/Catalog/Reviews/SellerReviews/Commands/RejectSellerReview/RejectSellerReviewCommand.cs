using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.RejectSellerReview;

public sealed record RejectSellerReviewCommand(Guid ReviewId) : IRequest<Result>;