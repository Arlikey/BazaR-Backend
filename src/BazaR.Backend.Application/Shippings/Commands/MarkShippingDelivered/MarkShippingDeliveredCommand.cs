using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.MarkShippingDelivered;

public sealed record MarkShippingDeliveredCommand(
    Guid ShippingId) : IRequest<Result>;