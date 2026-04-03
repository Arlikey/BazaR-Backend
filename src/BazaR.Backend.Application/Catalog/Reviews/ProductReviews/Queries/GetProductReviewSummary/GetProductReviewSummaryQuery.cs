
using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;
using MediatR;

namespace BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviewSummary;

public sealed record GetProductReviewSummaryQuery(Guid ProductId)
    : IRequest<ProductReviewSummaryDto?>;