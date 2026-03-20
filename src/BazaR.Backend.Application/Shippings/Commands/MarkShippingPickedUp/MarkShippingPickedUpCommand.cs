using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.MarkShippingPickedUp;

public sealed record MarkShippingPickedUpCommand(
    Guid ShippingId) : IRequest<Result>;