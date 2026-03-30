using BazaR.Backend.Application.Shippings.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.GetShippingByOrderId;

public sealed record GetShippingByOrderIdQuery(Guid OrderId)
    : IRequest<Result<ShippingDetailsDto>>;