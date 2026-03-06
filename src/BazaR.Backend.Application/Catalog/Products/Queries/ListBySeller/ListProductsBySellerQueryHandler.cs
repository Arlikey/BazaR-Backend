using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.ListBySeller;

public sealed class ListProductsBySellerQueryHandler
    : IRequestHandler<ListProductsBySellerQuery, Result<IReadOnlyList<ProductCardWithOfferDto>>>
{
    private readonly IProductReadRepository _read;
    private readonly IProductOfferAttacher _attacher;

    public ListProductsBySellerQueryHandler(
        IProductReadRepository read,
        IProductOfferAttacher attacher)
    {
        _read = read;
        _attacher = attacher;
    }

    public async Task<Result<IReadOnlyList<ProductCardWithOfferDto>>> Handle(
        ListProductsBySellerQuery request,
        CancellationToken ct)
    {
        if (request.SellerId == Guid.Empty)
            return Result<IReadOnlyList<ProductCardWithOfferDto>>
                .Failure(new Error("Seller.InvalidId", "SellerId is invalid."));

        var sellerId = new SellerId(request.SellerId);

        var limit = request.Limit is < 1 or > 100 ? 20 : request.Limit;

        
        var products = await _read.ListBySellerAsync(sellerId, limit, ct);

        
        var merged = await _attacher.AttachOffersAsync(products, ct);

        return Result<IReadOnlyList<ProductCardWithOfferDto>>.Success(merged);
    }
}