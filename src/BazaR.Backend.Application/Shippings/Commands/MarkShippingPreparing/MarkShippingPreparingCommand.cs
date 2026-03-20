using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.MarkShippingPreparing;

public sealed record MarkShippingPreparingCommand(
    Guid ShippingId) : IRequest<Result>;