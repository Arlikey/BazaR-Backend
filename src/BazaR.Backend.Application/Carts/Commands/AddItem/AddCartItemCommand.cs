using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.AddItem;

public sealed record AddCartItemCommand(
    Guid OfferId,
    int Quantity
) : IRequest<Result>;