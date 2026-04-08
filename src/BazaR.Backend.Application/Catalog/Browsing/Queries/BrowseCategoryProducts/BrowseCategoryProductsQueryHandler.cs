using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Browsing.Abstractions;
using BazaR.Backend.Application.Catalog.Browsing.DTOs;

using BazaR.Backend.Application.Catalog.Products.Services;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Browsing.Queries.BrowseCategoryProducts;

public sealed class BrowseCategoryProductsQueryHandler
    : IRequestHandler<BrowseCategoryProductsQuery, PagedResult<ProductCardWithOfferDto>>
{
    private readonly ICatalogBrowseReadRepository _catalogBrowseReadRepository;
    private readonly ProductOfferAttacher _productOfferAttacher;

    public BrowseCategoryProductsQueryHandler(
        ICatalogBrowseReadRepository catalogBrowseReadRepository,
        ProductOfferAttacher productOfferAttacher)
    {
        _catalogBrowseReadRepository = catalogBrowseReadRepository;
        _productOfferAttacher = productOfferAttacher;
    }

    public async Task<PagedResult<ProductCardWithOfferDto>> Handle(
        BrowseCategoryProductsQuery request,
        CancellationToken ct)
    {
        var products = await _catalogBrowseReadRepository.BrowseCategoryProductsAsync(
        request.CategoryId,
        request.Filters,
        request.SystemFilters,
        request.Page,
        request.PageSize,
        request.SortBy,
        ct);

        var itemsWithOffers = await _productOfferAttacher.AttachOffersAsync(products.Items, ct);

        return new PagedResult<ProductCardWithOfferDto>(
            itemsWithOffers,
            products.TotalCount,
            products.Page,
            products.PageSize);
    }
}