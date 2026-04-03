using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.ApproveProductReview;

public sealed record ApproveProductReviewCommand(Guid ReviewId) : IRequest<Result>;