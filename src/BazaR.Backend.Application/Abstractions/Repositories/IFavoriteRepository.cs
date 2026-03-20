using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Favorites;

using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IFavoriteRepository
{
    Task<bool> ExistsAsync(
        UserId userId,
        ProductId productId,
        CancellationToken ct);

    Task AddAsync(
        Favorite favorite,
        CancellationToken ct);

    Task RemoveAsync(
        UserId userId,
        ProductId productId,
        CancellationToken ct);

    Task<IReadOnlyList<ProductCardDto>> GetProductCardsAsync(
        UserId userId,
        int limit,
        CancellationToken ct);

    Task<HashSet<ProductId>> GetFavoriteProductIdsAsync(
        UserId userId,
        IReadOnlyCollection<ProductId> productIds,
        CancellationToken ct);

    Task<bool> IsFavoriteAsync(
        UserId userId,
        ProductId productId,
        CancellationToken ct);
}