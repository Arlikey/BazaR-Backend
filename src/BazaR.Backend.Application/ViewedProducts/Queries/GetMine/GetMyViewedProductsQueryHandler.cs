using BazaR.Backend.Application.Abstractions;
using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Application.Common.Abstractions;

using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Identity;
using MediatR;

namespace BazaR.Backend.Application.ViewedProducts.Queries.GetMine;

public sealed class GetMyViewedProductsQueryHandler
    : IRequestHandler<GetMyViewedProductsQuery, Result<PagedResult<ProductCardWithOfferDto>>>
{
    private readonly IViewedProductRepository _viewedProducts;
    private readonly IProductReadRepository _products;
    private readonly IProductOfferAttacher _attacher;
    private readonly ICurrentUser _currentUser;

    public GetMyViewedProductsQueryHandler(
        IViewedProductRepository viewedProducts,
        IProductReadRepository products,
        IProductOfferAttacher attacher,
        ICurrentUser currentUser)
    {
        _viewedProducts = viewedProducts;
        _products = products;
        _attacher = attacher;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<ProductCardWithOfferDto>>> Handle(
        GetMyViewedProductsQuery request,
        CancellationToken ct)
    {
        if (_currentUser.UserId == Guid.Empty)
            return Result<PagedResult<ProductCardWithOfferDto>>.Failure(AuthUserErrors.Unauthorized);

        var pagedIds = await _viewedProducts.GetPagedProductIdsByUserAsync(
            _currentUser.UserId,
            request.Page,
            request.PageSize,
            ct);

        if (pagedIds.Items.Count == 0)
        {
            var emptyResult = new PagedResult<ProductCardWithOfferDto>
            {
                Items = Array.Empty<ProductCardWithOfferDto>(),
                TotalCount = pagedIds.TotalCount,
                Page = pagedIds.Page,
                PageSize = pagedIds.PageSize
            };

            return Result<PagedResult<ProductCardWithOfferDto>>.Success(emptyResult);
        }

        var productCards = await _products.ListByIdsAsync(pagedIds.Items, ct);
        var merged = await _attacher.AttachOffersAsync(productCards, ct);

        var orderMap = pagedIds.Items
            .Select((id, index) => new { Id = id.Value, Index = index })
            .ToDictionary(x => x.Id, x => x.Index);

        var ordered = merged
            .OrderBy(x => orderMap[x.Id])
            .ToList();

        var result = new PagedResult<ProductCardWithOfferDto>
        {
            Items = ordered,
            TotalCount = pagedIds.TotalCount,
            Page = pagedIds.Page,
            PageSize = pagedIds.PageSize
        };

        return Result<PagedResult<ProductCardWithOfferDto>>.Success(result);
    }
}