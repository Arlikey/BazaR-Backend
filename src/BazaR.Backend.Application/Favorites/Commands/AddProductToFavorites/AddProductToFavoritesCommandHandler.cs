using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Favorites;

using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Favorites.Commands.AddProductToFavorites;

public sealed class AddProductToFavoritesCommandHandler
    : IRequestHandler<AddProductToFavoritesCommand, Result>
{
    private readonly IFavoriteRepository _favorites;
    private readonly IProductRepository _products;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;

    public AddProductToFavoritesCommandHandler(
        IFavoriteRepository favorites,
        IProductRepository products,
        ICurrentUser currentUser,
        IUnitOfWork uow)
    {
        _favorites = favorites;
        _products = products;
        _currentUser = currentUser;
        _uow = uow;
    }

    public async Task<Result> Handle(AddProductToFavoritesCommand request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var userId = new UserId(_currentUser.UserId);
        var productId = new ProductId(request.ProductId);

        var product = await _products.GetByIdAsync(productId, ct);
        if (product is null)
            return Result.Failure(new Error("Favorites.Product.NotFound", "Product not found."));

        var alreadyExists = await _favorites.ExistsAsync(userId, productId, ct);
        if (alreadyExists)
            return Result.Success(); // идемпотентно

        var favorite = Favorite.Create(userId, productId, DateTime.UtcNow);

        await _favorites.AddAsync(favorite, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}