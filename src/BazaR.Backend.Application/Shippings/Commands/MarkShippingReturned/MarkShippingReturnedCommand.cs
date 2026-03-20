using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.MarkShippingReturned;

public sealed record MarkShippingReturnedCommand(
    Guid ShippingId) : IRequest<Result>;