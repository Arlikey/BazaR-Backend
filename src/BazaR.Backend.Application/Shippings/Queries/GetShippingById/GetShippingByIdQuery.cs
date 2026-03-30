using BazaR.Backend.Application.Shippings.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.GetShippingById;

public sealed record GetShippingByIdQuery(Guid ShippingId)
    : IRequest<Result<ShippingDetailsDto>>;