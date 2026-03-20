using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Favorites.Queries.GetMyFavorites;

public sealed class GetMyFavoritesQueryHandler
    : IRequestHandler<GetMyFavoritesQuery, IReadOnlyList<ProductCardWithOfferDto>>
{
    private readonly IFavoriteRepository _favorites;
    private readonly IProductOfferAttacher _attacher;
    private readonly ICurrentUser _currentUser;

    public GetMyFavoritesQueryHandler(
        IFavoriteRepository favorites,
        IProductOfferAttacher attacher,
        ICurrentUser currentUser)
    {
        _favorites = favorites;
        _attacher = attacher;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ProductCardWithOfferDto>> Handle(
        GetMyFavoritesQuery request,
        CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated)
            return Array.Empty<ProductCardWithOfferDto>();

        var userId = new UserId(_currentUser.UserId);

        var products = await _favorites.GetProductCardsAsync(userId, request.Limit, ct);

        if (products.Count == 0)
            return Array.Empty<ProductCardWithOfferDto>();

        var merged = await _attacher.AttachOffersAsync(products, ct);

        return merged;
    }
}