using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Common;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Orders.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryHandler
    : IRequestHandler<GetMyOrdersQuery, Result<PagedResult<OrderListItemDto>>>
{
    private readonly IOrderReadRepository _orders;
    private readonly ICurrentUser _current;

    public GetMyOrdersQueryHandler(
        IOrderReadRepository orders,
        ICurrentUser current)
    {
        _orders = orders;
        _current = current;
    }

    public async Task<Result<PagedResult<OrderListItemDto>>> Handle(
        GetMyOrdersQuery request,
        CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
        {
            return Result<PagedResult<OrderListItemDto>>.Failure(
                new Error("Auth.Required", "Authentication required."));
        }

        var filter = new OrderListFilter(
            request.Query,
            request.Tab,
            request.Page,
            request.PageSize);

        var result = await _orders.GetBuyerPagedAsync(_current.UserId, filter, ct);

        return Result<PagedResult<OrderListItemDto>>.Success(result);
    }
}