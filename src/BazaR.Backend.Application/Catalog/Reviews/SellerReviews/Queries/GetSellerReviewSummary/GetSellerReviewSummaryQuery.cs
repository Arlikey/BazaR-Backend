using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;

using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviewSummary;

public sealed record GetSellerReviewSummaryQuery(Guid SellerId)
    : IRequest<SellerReviewSummaryDto?>;