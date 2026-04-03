using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.DeleteProductReviewByAdmin;

public sealed record DeleteProductReviewByAdminCommand(Guid ReviewId) : IRequest<Result>;