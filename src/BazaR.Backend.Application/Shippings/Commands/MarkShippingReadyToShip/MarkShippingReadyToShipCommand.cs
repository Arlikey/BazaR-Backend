using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.MarkShippingReadyToShip;

public sealed record MarkShippingReadyToShipCommand(
    Guid ShippingId) : IRequest<Result>;