using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.RemoveItem;

public sealed record RemoveCartItemCommand(Guid OfferId) : IRequest<Result>;