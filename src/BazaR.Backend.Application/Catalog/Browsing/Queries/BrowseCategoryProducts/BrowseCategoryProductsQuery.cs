/*using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Browsing.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Browsing.Queries.BrowseCategoryProducts;

public sealed record BrowseCategoryProductsQuery(
    Guid CategoryId,
    IReadOnlyCollection<CatalogSelectedFilterDto> Filters,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null)
    : IRequest<PagedResult<ProductCardWithOfferDto>>;*/


using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Browsing.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Browsing.Queries.BrowseCategoryProducts;

public sealed record BrowseCategoryProductsQuery(
    Guid CategoryId,
    IReadOnlyCollection<CatalogSelectedFilterDto> Filters,
    CatalogSystemFiltersDto? SystemFilters,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null)
    : IRequest<PagedResult<ProductCardWithOfferDto>>;