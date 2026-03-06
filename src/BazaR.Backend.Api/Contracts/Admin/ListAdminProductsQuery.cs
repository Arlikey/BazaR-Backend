using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using MediatR;

public sealed record ListAdminProductsQuery(
    Guid? SellerId,
    ProductStatus? Status,
    string? Search,
    int Page,
    int PageSize
) : IRequest<Result<PagedResult<ProductListItemDto>>>;