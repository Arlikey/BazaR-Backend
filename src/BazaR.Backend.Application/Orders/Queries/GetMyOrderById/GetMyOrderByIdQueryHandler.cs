using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Orders.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Orders.Queries.GetMyOrderById;

public sealed class GetMyOrderByIdQueryHandler
    : IRequestHandler<GetMyOrderByIdQuery, Result<OrderDetailsDto>>
{
    private readonly IOrderReadRepository _orders;
    private readonly ICurrentUser _current;

    public GetMyOrderByIdQueryHandler(
        IOrderReadRepository orders,
        ICurrentUser current)
    {
        _orders = orders;
        _current = current;
    }

    public async Task<Result<OrderDetailsDto>> Handle(
        GetMyOrderByIdQuery request,
        CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
        {
            return Result<OrderDetailsDto>.Failure(
                new Error("Auth.Required", "Authentication required."));
        }

        var order = await _orders.GetBuyerOrderByIdAsync(_current.UserId, request.OrderId, ct);
        if (order is null)
        {
            return Result<OrderDetailsDto>.Failure(
                new Error("Order.NotFound", "Order was not found."));
        }

        return Result<OrderDetailsDto>.Success(order);
    }
}