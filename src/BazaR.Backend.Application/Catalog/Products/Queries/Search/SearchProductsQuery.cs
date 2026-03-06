using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.Search;

public sealed record SearchProductsQuery(ProductSearchFilter Filter, Pagination Pagination)
    : IRequest<Result<PagedResult<ProductCardWithOfferDto>>>;