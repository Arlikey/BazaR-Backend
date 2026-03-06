using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.ListByCategory;

public sealed class ListProductsByCategoryQueryHandler
    : IRequestHandler<ListProductsByCategoryQuery, Result<IReadOnlyList<ProductCardWithOfferDto>>>
{
    private readonly IProductReadRepository _read;
    private readonly IProductOfferAttacher _attacher;

    public ListProductsByCategoryQueryHandler(
        IProductReadRepository read,
        IProductOfferAttacher attacher)
    {
        _read = read;
        _attacher = attacher;
    }

    public async Task<Result<IReadOnlyList<ProductCardWithOfferDto>>> Handle(
        ListProductsByCategoryQuery request,
        CancellationToken ct)
    {
        
        var products = await _read.ListByCategoryAsync(
            request.CategoryId,
            request.Status,
            ct);

       
        var merged = await _attacher.AttachOffersAsync(products, ct);

        return Result<IReadOnlyList<ProductCardWithOfferDto>>.Success(merged);
    }
}