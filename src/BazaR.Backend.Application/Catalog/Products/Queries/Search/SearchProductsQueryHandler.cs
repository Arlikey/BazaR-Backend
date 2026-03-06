using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.Search;

public sealed class SearchProductsQueryHandler
    : IRequestHandler<SearchProductsQuery, Result<PagedResult<ProductCardWithOfferDto>>>
{
    private readonly IProductReadRepository _products;
    private readonly IProductOfferAttacher _attacher;

    public SearchProductsQueryHandler(
        IProductReadRepository products,
        IProductOfferAttacher attacher)
    {
        _products = products;
        _attacher = attacher;
    }

    public async Task<Result<PagedResult<ProductCardWithOfferDto>>> Handle(
        SearchProductsQuery request,
        CancellationToken ct)
    {
        // Нормализуем поисковый запрос
        var term = (request.Filter.Term ?? "").Trim();
        var page = request.Pagination.SafePage;
        var pageSize = request.Pagination.SafePageSize;

        // Если запрос пустой – возвращаем пустой результат
        if (string.IsNullOrWhiteSpace(term))
        {
            return Result<PagedResult<ProductCardWithOfferDto>>.Success(
                new PagedResult<ProductCardWithOfferDto>
                {
                    Items = Array.Empty<ProductCardWithOfferDto>(),
                    TotalCount = 0,
                    Page = page,
                    PageSize = pageSize
                });
        }

        var filter = request.Filter with { Term = term };
        var pagination = new Pagination(page, pageSize);

        // Получаем страницу продуктов по поисковому запросу
        var productPage = await _products.SearchAsync(filter, pagination, ct);

        // Присоединяем к каждому продукту подходящий оффер
        var merged = await _attacher.AttachOffersAsync(productPage.Items, ct);

        // Формируем и возвращаем результат с пагинацией
        return Result<PagedResult<ProductCardWithOfferDto>>.Success(
            new PagedResult<ProductCardWithOfferDto>
            {
                Items = merged,
                TotalCount = productPage.TotalCount,
                Page = page,
                PageSize = pageSize
            });
    }
}