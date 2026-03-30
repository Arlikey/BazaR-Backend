using BazaR.Backend.Application.Common;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Application.Shippings.DTOs;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface IShippingReadRepository
{
    Task<ShippingDetailsDto?> GetByIdAsync(Guid shippingId, CancellationToken ct = default);

    Task<ShippingDetailsDto?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);

    Task<PagedResult<ShippingListItemDto>> GetPagedAsync(
        ShippingListFilter filter,
        CancellationToken ct = default);

    Task<PagedResult<ShippingListItemDto>> GetSellerPagedAsync(
        Guid sellerId,
        ShippingListFilter filter,
        CancellationToken ct = default);

    Task<PagedResult<ShippingListItemDto>> GetCustomerPagedAsync(
        Guid customerId,
        ShippingListFilter filter,
        CancellationToken ct = default);
}