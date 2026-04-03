using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.RejectProductReview;

public sealed record RejectProductReviewCommand(Guid ReviewId) : IRequest<Result>;