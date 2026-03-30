using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Sellers.DTOs;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Queries.ListBrands;

public sealed class ListBrandsQueryHandler : IRequestHandler<ListBrandsQuery, PagedResult<BrandListItemDto>>
{
    private readonly IBrandReadRepository _brands;

    public ListBrandsQueryHandler(IBrandReadRepository brands)
    {
        _brands = brands;
    }

    public Task<PagedResult<BrandListItemDto>> Handle(ListBrandsQuery request, CancellationToken ct)
    {
        return _brands.GetPagedAsync(
            request.Search,
            request.Status,
            request.Page,
            request.PageSize,
            ct);
    }
}