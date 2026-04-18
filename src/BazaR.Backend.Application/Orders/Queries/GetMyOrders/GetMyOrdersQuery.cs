using BazaR.Backend.Application.Common;
using BazaR.Backend.Application.Orders.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Orders.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery(
    string? Query,
    string? Tab,
    int Page = 1,
    int PageSize = 20)
    : IRequest<Result<PagedResult<OrderListItemDto>>>;