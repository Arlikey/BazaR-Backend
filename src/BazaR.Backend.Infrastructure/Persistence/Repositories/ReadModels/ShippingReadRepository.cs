using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Common;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Application.Shippings.DTOs;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Shippings;
using BazaR.Backend.Domain.Users;
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
        var query = _db.Shippings
            .AsNoTracking();

        query = ApplyFilter(query, filter);

        return await ToPagedResultAsync(query, filter, ct);
    }

    public async Task<PagedResult<ShippingListItemDto>> GetSellerPagedAsync(
        Guid sellerId,
        ShippingListFilter filter,
        CancellationToken ct = default)
    {
        var query = _db.Shippings
            .AsNoTracking()
            .Where(x => x.SellerId == new SellerId(sellerId));

        query = ApplyFilter(query, filter);

        return await ToPagedResultAsync(query, filter, ct);
    }

    public async Task<PagedResult<ShippingListItemDto>> GetCustomerPagedAsync(
        Guid customerId,
        ShippingListFilter filter,
        CancellationToken ct = default)
    {
        var query = _db.Shippings
            .AsNoTracking()
            .Where(x => x.CustomerId == new UserId(customerId));

        query = ApplyFilter(query, filter);

        return await ToPagedResultAsync(query, filter, ct);
    }

    private static IQueryable<Shipping> ApplyFilter(
        IQueryable<Shipping> query,
        ShippingListFilter filter)
    {
        if (filter is null)
            return query.OrderByDescending(x => x.CreatedAtUtc);

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var q = filter.Query.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.Recipient.FirstName + " " + x.Recipient.LastName, $"%{q}%") ||
                EF.Functions.ILike(x.Recipient.Phone, $"%{q}%") ||
                EF.Functions.ILike(x.Destination.City, $"%{q}%") ||
                (x.Destination.PickupPointName != null && EF.Functions.ILike(x.Destination.PickupPointName, $"%{q}%")) ||
                (x.TrackingNumber != null && EF.Functions.ILike(x.TrackingNumber, $"%{q}%")));
        }

        if (filter.Method.HasValue)
        {
            var method = (ShippingMethod)filter.Method.Value;
            query = query.Where(x => x.Method == method);
        }

        if (filter.Status.HasValue)
        {
            var status = (ShippingStatus)filter.Status.Value;
            query = query.Where(x => x.Status == status);
        }

        return query.OrderByDescending(x => x.CreatedAtUtc);
    }

    private static async Task<PagedResult<ShippingListItemDto>> ToPagedResultAsync(
        IQueryable<Shipping> query,
        ShippingListFilter filter,
        CancellationToken ct)
    {
        var page = filter?.Page > 0 ? filter.Page : 1;
        var pageSize = filter?.PageSize > 0 ? filter.PageSize : 20;

        if (pageSize > 100)
            pageSize = 100;

        var totalCount = await query.CountAsync(ct);

        var entities = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = entities
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
            ))
            .ToList();

        return new PagedResult<ShippingListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
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
}