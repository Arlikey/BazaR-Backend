using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.ApproveSellerReview;

public sealed record ApproveSellerReviewCommand(Guid ReviewId) : IRequest<Result>;