using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.SetShippingSender;

public sealed record SetShippingSenderCommand(
    Guid ShippingId,
    string Name,
    string Phone,
    string CountryCode,
    string? PickupPointCode,
    string? PickupPointName
) : IRequest<Result>;