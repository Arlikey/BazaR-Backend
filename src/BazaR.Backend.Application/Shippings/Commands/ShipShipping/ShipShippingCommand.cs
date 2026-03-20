using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.ShipShipping;

public sealed record ShipShippingCommand(
    Guid ShippingId,
    string Carrier,
    string TrackingNumber,
    string? TrackingUrl,
    string? ExternalShipmentId) : IRequest<Result>;