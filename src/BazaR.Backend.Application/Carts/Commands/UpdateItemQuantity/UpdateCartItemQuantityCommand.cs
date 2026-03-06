using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.UpdateItemQuantity;

public sealed record UpdateCartItemQuantityCommand(
    Guid OfferId,
    int Quantity
) : IRequest<Result>;