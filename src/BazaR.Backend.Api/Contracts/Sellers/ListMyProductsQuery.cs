using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Api.Contracts.Sellers
{
    public sealed record ListMyProductsQuery(
    ProductStatus? Status,
    string? Search,
    int Page,
    int PageSize
) : IRequest<Result<PagedResult<ProductListItemDto>>>;
}
