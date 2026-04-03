
using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;
using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviewById;

public sealed record GetSellerReviewByIdQuery(Guid ReviewId)
    : IRequest<SellerReviewDetailsDto?>;