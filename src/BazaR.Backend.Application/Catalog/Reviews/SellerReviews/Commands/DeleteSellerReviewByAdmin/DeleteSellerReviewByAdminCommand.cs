using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.DeleteSellerReviewByAdmin;

public sealed record DeleteSellerReviewByAdminCommand(Guid ReviewId) : IRequest<Result>;