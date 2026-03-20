using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Favorites.Commands.AddProductToFavorites;

public sealed record AddProductToFavoritesCommand(Guid ProductId) : IRequest<Result>;