using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.CancelShipping;

public sealed record CancelShippingCommand(
    Guid ShippingId) : IRequest<Result>;