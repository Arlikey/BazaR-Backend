using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.MarkShippingReadyForPickup;

public sealed record MarkShippingReadyForPickupCommand(Guid ShippingId) : IRequest<Result>;