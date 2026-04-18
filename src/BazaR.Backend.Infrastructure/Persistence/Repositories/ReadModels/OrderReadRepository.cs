using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Common;
using BazaR.Backend.Application.Orders.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Users;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class OrderReadRepository : IOrderReadRepository
{
    private readonly AppDbContext _db;

    public OrderReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<OrderListItemDto>> GetBuyerPagedAsync(
        Guid buyerUserId,
        OrderListFilter filter,
        CancellationToken ct = default)
    {
        var query = _db.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.BuyerUserId == new UserId(buyerUserId));

        query = ApplyFilter(query, filter);

        var page = filter?.Page > 0 ? filter.Page : 1;
        var pageSize = filter?.PageSize > 0 ? filter.PageSize : 20;

        if (pageSize > 100)
            pageSize = 100;

        var totalCount = await query.CountAsync(ct);

        var orders = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var productImages = await LoadProductImagesAsync(orders, ct);

        var items = orders
            .Select(order => MapListItem(order, productImages))
            .ToList();

        return new PagedResult<OrderListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OrderDetailsDto?> GetBuyerOrderByIdAsync(
        Guid buyerUserId,
        Guid orderId,
        CancellationToken ct = default)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.Id == new OrderId(orderId) &&
                     x.BuyerUserId == new UserId(buyerUserId),
                ct);

        if (order is null)
            return null;

        var productImages = await LoadProductImagesAsync(new[] { order }, ct);

        return MapDetails(order, productImages);
    }

    private static IQueryable<Order> ApplyFilter(
        IQueryable<Order> query,
        OrderListFilter filter)
    {
        if (filter is null)
            return query;

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var q = filter.Query.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(x.Customer.FirstName + " " + x.Customer.LastName, $"%{q}%") ||
                EF.Functions.ILike(x.Customer.Phone, $"%{q}%") ||
                (x.Customer.Email != null && EF.Functions.ILike(x.Customer.Email, $"%{q}%")));
        }

        if (!string.IsNullOrWhiteSpace(filter.Tab))
        {
            var tab = filter.Tab.Trim().ToLowerInvariant();

            if (tab == "current")
            {
                query = query.Where(x =>
                    x.Status != OrderStatus.Delivered &&
                    x.Status != OrderStatus.Completed &&
                    x.Status != OrderStatus.Cancelled);
            }
            else if (tab == "completed")
            {
                query = query.Where(x =>
                    x.Status == OrderStatus.Delivered ||
                    x.Status == OrderStatus.Completed);
            }
        }

        return query;
    }

    private async Task<Dictionary<Guid, string?>> LoadProductImagesAsync(
    IEnumerable<Order> orders,
    CancellationToken ct)
    {
        var productIds = orders
            .SelectMany(x => x.Items)
            .Select(x => x.ProductId.Value)
            .Distinct()
            .ToHashSet();

        if (productIds.Count == 0)
            return new Dictionary<Guid, string?>();

        var images = await _db.ProductImages
            .AsNoTracking()
            .Where(x => x.IsMain)
            .ToListAsync(ct);

        return images
            .Where(x => productIds.Contains(x.ProductId.Value)) 
            .ToDictionary(
                x => x.ProductId.Value,
                x => x.Url
            );
    }

    private static OrderListItemDto MapListItem(
        Order order,
        IReadOnlyDictionary<Guid, string?> productImages)
    {
        var previewItem = order.Items.FirstOrDefault();

        string? previewImageUrl = null;
        if (previewItem is not null)
        {
            productImages.TryGetValue(previewItem.ProductId.Value, out previewImageUrl);
        }

        var totalAmount = order.Items.Sum(x => x.PriceSnapshot.Amount * x.ActiveQuantity);
        var currency = order.Items.FirstOrDefault()?.PriceSnapshot.Currency ?? "UAH";

        return new OrderListItemDto(
            order.Id.Value,
            order.Id.Value.ToString()[..8],
            (int)order.Status,
            order.Status.ToString(),
            totalAmount,
            currency,
            order.Items.Count,
            previewImageUrl,
            order.CreatedAtUtc,
            order.DeliveredAtUtc,
            order.CompletedAtUtc
        );
    }

    private static OrderDetailsDto MapDetails(
        Order order,
        IReadOnlyDictionary<Guid, string?> productImages)
    {
        var subtotalAmount = order.Items.Sum(x => x.PriceSnapshot.Amount * x.ActiveQuantity);
        var currency = order.Items.FirstOrDefault()?.PriceSnapshot.Currency ?? "UAH";

        return new OrderDetailsDto(
            order.Id.Value,
            order.Id.Value.ToString()[..8],
            (int)order.Status,
            order.Status.ToString(),

            subtotalAmount,
            0m,
            0m,
            subtotalAmount,
            currency,

            order.Customer.FirstName,
            order.Customer.LastName,
            order.Customer.Phone,
            order.Customer.Email,

            order.Delivery.Method.ToString(),
            order.Delivery.City,
            null,
            order.Delivery.Warehouse,
            order.Delivery.Street,
            order.Delivery.Building,
            order.Delivery.Apartment,
            order.Delivery.PostalCode,

            order.CustomerComment,
            order.CreatedAtUtc,
            order.PaidAtUtc,
            order.DeliveredAtUtc,
            order.CompletedAtUtc,

            order.Items.Select(x =>
            {
                productImages.TryGetValue(x.ProductId.Value, out var imageUrl);

                return new OrderItemDto(
                    x.ProductId.Value,
                    x.ProductName,
                    x.Sku,
                    x.ActiveQuantity,
                    x.PriceSnapshot.Amount,
                    x.PriceSnapshot.Currency,
                    imageUrl
                );
            }).ToList()
        );
    }
}