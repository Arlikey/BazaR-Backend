using BazaR.Backend.Application.Catalog.Offers.DTOs;
using BazaR.Backend.Application.Common;
using BazaR.Backend.Application.Orders.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface IOrderReadRepository
{
    Task<PagedResult<OrderListItemDto>> GetBuyerPagedAsync(
        Guid buyerUserId,
        OrderListFilter filter,
        CancellationToken ct = default);

    Task<OrderDetailsDto?> GetBuyerOrderByIdAsync(
        Guid buyerUserId,
        Guid orderId,
        CancellationToken ct = default);
}