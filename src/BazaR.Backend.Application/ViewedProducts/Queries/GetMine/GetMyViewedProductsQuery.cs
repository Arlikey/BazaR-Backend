using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Products.DTOs;

using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.ViewedProducts.Queries.GetMine;

public sealed record GetMyViewedProductsQuery(
    int Page = 1,
    int PageSize = 10)
    : IRequest<Result<PagedResult<ProductCardWithOfferDto>>>;