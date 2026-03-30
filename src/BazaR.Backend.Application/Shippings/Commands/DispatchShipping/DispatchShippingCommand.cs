using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.DispatchShipping;

public sealed record DispatchShippingCommand(
    Guid ShippingId,
    string TrackingNumber
) : IRequest<Result>;