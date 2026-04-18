using BazaR.Backend.Application.Orders.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Orders.Queries.GetMyOrderById;

public sealed record GetMyOrderByIdQuery(Guid OrderId)
    : IRequest<Result<OrderDetailsDto>>;