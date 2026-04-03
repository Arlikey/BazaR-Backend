using BazaR.Backend.Application.Abstractions.Repositories.ReadModels;
using BazaR.Backend.Application.Carts.DTOs;
using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class CartReadRepository : ICartReadRepository
{
    private readonly AppDbContext _db;

    public CartReadRepository(AppDbContext db) => _db = db;

    public async Task<CustomerCartDto?> GetCustomerActiveCartAsync(Guid userId, CancellationToken ct = default)
    {
        var userVo = new UserId(userId);

        var query =
            from c in _db.Carts.AsNoTracking()
            where c.UserId == userVo && c.Status == CartStatus.Active
            select new CustomerCartDto(
                c.Id.Value,
                c.Status.ToString(),
                c.Currency,

                _db.Set<CartItem>().Count(i => i.CartId == c.Id),

                _db.Set<CartItem>()
                    .Where(i => i.CartId == c.Id)
                    .Sum(i => (int?)i.Quantity) ?? 0,

                _db.Set<CartItem>()
                    .Where(i => i.CartId == c.Id)
                    .Sum(i => (decimal?)(i.PriceSnapshot.Amount * i.Quantity)) ?? 0m,

                c.CreatedAt,
                c.UpdatedAt,

                (
                    from i in _db.Set<CartItem>()
                    join o in _db.Offers.AsNoTracking() on i.OfferId equals o.Id
                    join p in _db.Products.AsNoTracking() on o.ProductId equals p.Id
                    join s in _db.Sellers.AsNoTracking() on o.SellerId equals s.Id
                    where i.CartId == c.Id
                    let mainImage = p.Images
                        .OrderByDescending(img => img.IsMain)
                        .ThenBy(img => img.SortOrder)
                        .Select(img => img.Url)
                        .FirstOrDefault()
                    orderby i.AddedAt descending   
                    select new CustomerCartItemDto(
                        i.OfferId.Value,
                        p.Name,
                        p.Slug != null ? p.Slug.Value : null,
                        mainImage,
                        p.Description,
                        s.Name,
                        i.Quantity,
                        i.PriceSnapshot.Amount,
                        i.PriceSnapshot.Currency,
                        i.Quantity * i.PriceSnapshot.Amount
                    )
                ).ToList()
            );

        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<CartReadModel?> GetByIdAsync(Guid cartId, CancellationToken ct = default)
    {
        var cartVo = new CartId(cartId);

        return await _db.Carts
            .AsNoTracking()
            .Where(c => c.Id == cartVo)
            .Select(c => new CartReadModel
            {
                Id = c.Id.Value,
                UserId = c.UserId.Value,
                Status = c.Status.ToString(),
                Currency = c.Currency,

                ItemsCount = _db.Set<CartItem>().Count(i => i.CartId == c.Id),

                TotalQuantity = _db.Set<CartItem>()
                    .Where(i => i.CartId == c.Id)
                    .Sum(i => (int?)i.Quantity) ?? 0,

                TotalAmount = _db.Set<CartItem>()
                    .Where(i => i.CartId == c.Id)
                    .Sum(i => (decimal?)(i.PriceSnapshot.Amount * i.Quantity)) ?? 0m,

                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,

                Items = _db.Set<CartItem>()
                    .Where(i => i.CartId == c.Id)
                    .OrderByDescending(i => i.AddedAt)   
                    .Select(i => new CartItemReadModel
                    {
                        OfferId = i.OfferId.Value,
                        Quantity = i.Quantity,
                        PriceAmount = i.PriceSnapshot.Amount,
                        Currency = i.PriceSnapshot.Currency,
                        UpdatedAt = i.UpdatedAt
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<CartPagedResult<CartAdminListItemReadModel>> SearchAsync(
        CartFilter filter,
        Pagination pagination,
        CancellationToken ct = default)
    {
        var query = _db.Carts.AsNoTracking().AsQueryable();

        if (filter.UserId is not null && filter.UserId != Guid.Empty)
        {
            var userVo = new UserId(filter.UserId.Value);
            query = query.Where(c => c.UserId == userVo);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status) &&
            Enum.TryParse<CartStatus>(filter.Status, true, out var st))
        {
            query = query.Where(c => c.Status == st);
        }

        query = query
            .OrderByDescending(c => c.UpdatedAt)
            .ThenByDescending(c => c.Id);

        var page = pagination.Page < 1 ? 1 : pagination.Page;
        var pageSize = pagination.PageSize is < 1 or > 200 ? 50 : pagination.PageSize;

        var total = await query.LongCountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CartAdminListItemReadModel
            {
                Id = c.Id.Value,
                UserId = c.UserId.Value,
                Status = c.Status.ToString(),
                Currency = c.Currency,

                ItemsCount = _db.Set<CartItem>().Count(i => i.CartId == c.Id),

                TotalQuantity = _db.Set<CartItem>()
                    .Where(i => i.CartId == c.Id)
                    .Sum(i => (int?)i.Quantity) ?? 0,

                TotalAmount = _db.Set<CartItem>()
                    .Where(i => i.CartId == c.Id)
                    .Sum(i => (decimal?)(i.PriceSnapshot.Amount * i.Quantity)) ?? 0m,

                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(ct);

        return new CartPagedResult<CartAdminListItemReadModel>(
            items,
            total,
            page,
            pageSize
        );
    }
}