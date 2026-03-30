using BazaR.Backend.Application.Abstractions.ReadModels;

using BazaR.Backend.Application.Sellers.DTOs;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Queries.ListBrands;

public sealed record ListBrandsQuery(
    string? Search,
    string? Status,
    int Page,
    int PageSize) : IRequest<PagedResult<BrandListItemDto>>;