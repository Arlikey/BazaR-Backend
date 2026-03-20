using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;

using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Favorites.Commands.RemoveProductFromFavorites;

public sealed class RemoveProductFromFavoritesCommandHandler
    : IRequestHandler<RemoveProductFromFavoritesCommand, Result>
{
    private readonly IFavoriteRepository _favorites;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uow;

    public RemoveProductFromFavoritesCommandHandler(
        IFavoriteRepository favorites,
        ICurrentUser currentUser,
        IUnitOfWork uow)
    {
        _favorites = favorites;
        _currentUser = currentUser;
        _uow = uow;
    }

    public async Task<Result> Handle(RemoveProductFromFavoritesCommand request, CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var userId = new UserId(_currentUser.UserId);
        var productId = new ProductId(request.ProductId);

        await _favorites.RemoveAsync(userId, productId, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}