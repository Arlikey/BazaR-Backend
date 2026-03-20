using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Favorites.Commands.RemoveProductFromFavorites;

public sealed record RemoveProductFromFavoritesCommand(Guid ProductId) : IRequest<Result>;