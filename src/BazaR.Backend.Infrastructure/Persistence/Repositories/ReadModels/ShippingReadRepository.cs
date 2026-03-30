using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Common;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Application.Shippings.DTOs;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Shippings;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class ShippingReadRepository : IShippingReadRepository
{
    private readonly AppDbContext _db;

    public ShippingReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ShippingDetailsDto?> GetByIdAsync(
        Guid shippingId,
        CancellationToken ct = default)
    {
        var shipping = await _db.Shippings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == new ShippingId(shippingId), ct);

        if (shipping is null)
            return null;

        return MapDetails(shipping);
    }

    public async Task<ShippingDetailsDto?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken ct = default)
    {
        var shipping = await _db.Shippings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrderId == new OrderId(orderId), ct);

        if (shipping is null)
            return null;

        return MapDetails(shipping);
    }

    public async Task<PagedResult<ShippingListItemDto>> GetPagedAsync(
        ShippingListFilter filter,
        CancellationToken ct = default)
    {
        var query = BuildListQuery();
        query = ApplyFilter(query, filter);

        return await ToPagedResultAsync(query, filter, ct);
    }

    public async Task<PagedResult<ShippingListItemDto>> GetSellerPagedAsync(
        Guid sellerId,
        ShippingListFilter filter,
        CancellationToken ct = default)
    {
        var query = BuildListQuery()
            .Where(x => x.SellerId == sellerId);

        query = ApplyFilter(query, filter);

        return await ToPagedResultAsync(query, filter, ct);
    }

    public async Task<PagedResult<ShippingListItemDto>> GetCustomerPagedAsync(
        Guid customerId,
        ShippingListFilter filter,
        CancellationToken ct = default)
    {
        var query = BuildListQuery()
            .Where(x => x.CustomerId == customerId);

        query = ApplyFilter(query, filter);

        return await ToPagedResultAsync(query, filter, ct);
    }

    private IQueryable<ShippingListItemDto> BuildListQuery()
    {
        return _db.Shippings
            .AsNoTracking()
            .Select(x => new ShippingListItemDto(
                x.Id.Value,
                x.OrderId.Value,
                x.SellerId.Value,
                x.CustomerId.Value,
                (int)x.Method,
                (int)x.SettlementMode,
                (int)x.Status,
                x.Recipient.FirstName + " " + x.Recipient.LastName,
                x.Recipient.Phone,
                x.Destination.City,
                x.Destination.PickupPointName,
                x.TrackingNumber,
                x.CreatedAtUtc,
                x.DispatchedAtUtc,
                x.DeliveredAtUtc
            ));
    }

    private static ShippingDetailsDto MapDetails(Shipping shipping)
    {
        return new ShippingDetailsDto(
            shipping.Id.Value,
            shipping.OrderId.Value,
            shipping.SellerId.Value,
            shipping.CustomerId.Value,
            (int)shipping.Method,
            (int)shipping.SettlementMode,
            (int)shipping.Status,

            shipping.Recipient.FirstName,
            shipping.Recipient.LastName,
            shipping.Recipient.FirstName + " " + shipping.Recipient.LastName,
            shipping.Recipient.Phone,
            shipping.Recipient.Email,

            shipping.Destination.Country,
            shipping.Destination.Region,
            shipping.Destination.City,
            shipping.Destination.Street,
            shipping.Destination.House,
            shipping.Destination.Apartment,
            shipping.Destination.PostalCode,
            shipping.Destination.PickupPointCode,
            shipping.Destination.PickupPointName,

            shipping.Sender?.Name,
            shipping.Sender?.Phone,
            shipping.Sender?.CountryCode,
            shipping.Sender?.PickupPointCode,
            shipping.Sender?.PickupPointName,

            shipping.TrackingNumber,
            shipping.Parcels
                .OrderBy(p => p.RowNumber)
                .Select(p => new ShippingParcelDto(
                    p.RowNumber,
                    p.CargoCategory,
                    p.Description,
                    p.InsuranceCost,
                    p.Width,
                    p.Length,
                    p.Height,
                    p.ActualWeight,
                    p.VolumetricWeight
                ))
                .ToList(),

            shipping.CreatedAtUtc,
            shipping.UpdatedAtUtc,
            shipping.DispatchedAtUtc,
            shipping.ReadyForPickupAtUtc,
            shipping.DeliveredAtUtc,
            shipping.CancelledAtUtc
        );
    }

    private static IQueryable<ShippingListItemDto> ApplyFilter(
        IQueryable<ShippingListItemDto> query,
        ShippingListFilter filter)
    {
        if (filter is null)
            return query.OrderByDescending(x => x.CreatedAtUtc);

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var q = filter.Query.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.RecipientFullName, $"%{q}%") ||
                EF.Functions.ILike(x.RecipientPhone, $"%{q}%") ||
                EF.Functions.ILike(x.City, $"%{q}%") ||
                (x.PickupPointName != null && EF.Functions.ILike(x.PickupPointName, $"%{q}%")) ||
                (x.TrackingNumber != null && EF.Functions.ILike(x.TrackingNumber, $"%{q}%")));
        }

        if (filter.Method.HasValue)
        {
            query = query.Where(x => x.Method == filter.Method.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        return query.OrderByDescending(x => x.CreatedAtUtc);
    }

    private static async Task<PagedResult<ShippingListItemDto>> ToPagedResultAsync(
        IQueryable<ShippingListItemDto> query,
        ShippingListFilter filter,
        CancellationToken ct)
    {
        var page = filter?.Page > 0 ? filter.Page : 1;
        var pageSize = filter?.PageSize > 0 ? filter.PageSize : 20;

        if (pageSize > 100)
            pageSize = 100;

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<ShippingListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}