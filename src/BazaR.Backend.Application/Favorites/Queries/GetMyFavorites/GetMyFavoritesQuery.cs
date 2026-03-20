using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using MediatR;

namespace BazaR.Backend.Application.Favorites.Queries.GetMyFavorites;

public sealed record GetMyFavoritesQuery(int Limit = 20)
    : IRequest<IReadOnlyList<ProductCardWithOfferDto>>;