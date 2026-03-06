using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Offers.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class OfferReadRepository : IOfferReadRepository
{
    private readonly AppDbContext _db;

    public OfferReadRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<OfferCardDto>> GetByProductCardIdsAsync(
    IReadOnlyCollection<Guid> productIds,
    CancellationToken ct)
    {
        if (productIds == null || productIds.Count == 0)
            return Array.Empty<OfferCardDto>();

        var productVoIds = productIds
            .Select(id => new ProductId(id))
            .ToList();

        return await _db.Offers
            .AsNoTracking()
            .Where(o => productVoIds.Contains(o.ProductId))
            .Select(o => new OfferCardDto(
                o.ProductId.Value,
                o.Price != null ? o.Price.Amount : (decimal?)null,
                o.Price != null ? o.Price.Currency : null,
                o.OldPrice != null ? o.OldPrice.Amount : (decimal?)null,
                o.Stock > 0
            ))
            .ToListAsync(ct);
    }

    public Task<OfferDetailsDto?> GetByProductIdAsync(ProductId productId, CancellationToken ct)
    => _db.Offers
        .AsNoTracking()
        .Where(o => o.ProductId == productId)
        .Select(o => new OfferDetailsDto(
            o.Id.Value,
            o.ProductId.Value,
            o.SellerId.Value,
            o.Price != null ? o.Price.Amount : (decimal?)null,
            o.Price != null ? o.Price.Currency : null,
            o.OldPrice != null ? o.OldPrice.Amount : (decimal?)null,
            o.Stock,
            o.SellerSku,
            o.DeliveryDays,
            o.MinOrderQuantity,
            o.Status.ToString()
        ))
        .SingleOrDefaultAsync(ct);

    public async Task<OfferCatalogReadModel?> GetByProductIdAsync(
        Guid productId,
        CancellationToken ct)
    {
        return await _db.Offers
            .AsNoTracking()
            .Where(o => o.ProductId.Value == productId)
            .Select(o => new OfferCatalogReadModel(
                o.Id.Value,
                o.ProductId.Value,
                o.SellerId.Value,
                o.Price != null ? o.Price.Amount : (decimal?)null,
                o.Price != null ? o.Price.Currency : null,
                o.OldPrice != null ? o.OldPrice.Amount : (decimal?)null,
                o.Stock,
                o.SellerSku,
                o.DeliveryDays,
                o.MinOrderQuantity,
                o.Status.ToString()
            ))
            .FirstOrDefaultAsync(ct);
    }



    public async Task<OfferReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query =
            from o in _db.Offers.AsNoTracking()
            join p in _db.Products.AsNoTracking() on o.ProductId.Value equals p.Id.Value
            join s in _db.Sellers.AsNoTracking() on o.SellerId.Value equals s.Id.Value
            where o.Id.Value == id
            select new OfferReadModel
            {
                Id = o.Id.Value,
                ProductId = o.ProductId.Value,
                ProductName = p.Name,
                SellerId = o.SellerId.Value,
                SellerName = s.Name,

                PriceAmount = o.Price != null ? o.Price.Amount : (decimal?)null,
                PriceCurrency = o.Price != null ? o.Price.Currency : null,
                OldPriceAmount = o.OldPrice != null ? o.OldPrice.Amount : (decimal?)null,

                Stock = o.Stock,
                SellerSku = o.SellerSku,
                DeliveryDays = o.DeliveryDays,
                MinOrderQuantity = o.MinOrderQuantity,

                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /*public async Task<IReadOnlyList<OfferReadModel>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var query =
            from o in _db.Offers.AsNoTracking()
            join p in _db.Products.AsNoTracking() on o.ProductId.Value equals p.Id.Value
            join s in _db.Sellers.AsNoTracking() on o.SellerId.Value equals s.Id.Value
            where o.ProductId.Value == productId
            orderby o.CreatedAt descending
            select new OfferReadModel
            {
                Id = o.Id.Value,
                ProductId = o.ProductId.Value,
                ProductName = p.Name,
                SellerId = o.SellerId.Value,
                SellerName = s.Name,

                PriceAmount = o.Price != null ? o.Price.Amount : (decimal?)null,
                PriceCurrency = o.Price != null ? o.Price.Currency : null,
                OldPriceAmount = o.OldPrice != null ? o.OldPrice.Amount : (decimal?)null,

                Stock = o.Stock,
                SellerSku = o.SellerSku,
                DeliveryDays = o.DeliveryDays,
                MinOrderQuantity = o.MinOrderQuantity,

                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };

        return await query.ToListAsync(cancellationToken);
    }*/

    /*public async Task<IReadOnlyList<OfferReadModel>> GetBySellerAsync(Guid sellerId, CancellationToken cancellationToken = default)
    {
        var query =
            from o in _db.Offers.AsNoTracking()
            join p in _db.Products.AsNoTracking() on o.ProductId.Value equals p.Id.Value
            join s in _db.Sellers.AsNoTracking() on o.SellerId.Value equals s.Id.Value
            where o.SellerId.Value == sellerId
            orderby o.CreatedAt descending
            select new OfferReadModel
            {
                Id = o.Id.Value,
                ProductId = o.ProductId.Value,
                ProductName = p.Name,
                SellerId = o.SellerId.Value,
                SellerName = s.Name,

                PriceAmount = o.Price != null ? o.Price.Amount : (decimal?)null,
                PriceCurrency = o.Price != null ? o.Price.Currency : null,
                OldPriceAmount = o.OldPrice != null ? o.OldPrice.Amount : (decimal?)null,

                Stock = o.Stock,
                SellerSku = o.SellerSku,
                DeliveryDays = o.DeliveryDays,
                MinOrderQuantity = o.MinOrderQuantity,

                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };

        return await query.ToListAsync(cancellationToken);
    }*/

   /* public async Task<PagedResult<OfferReadModel>> SearchAsync(
        OfferFilter filter,
        Pagination pagination,
        CancellationToken cancellationToken = default)
    {
        var query =
            from o in _db.Offers.AsNoTracking()
            join p in _db.Products.AsNoTracking() on o.ProductId.Value equals p.Id.Value
            join s in _db.Sellers.AsNoTracking() on o.SellerId.Value equals s.Id.Value
            select new { o, p, s };

        // ---------- Filters ----------
        if (filter.ProductId.HasValue && filter.ProductId.Value != Guid.Empty)
        {
            var pid = filter.ProductId.Value;
            query = query.Where(x => x.o.ProductId.Value == pid);
        }

        if (filter.SellerId.HasValue && filter.SellerId.Value != Guid.Empty)
        {
            var sid = filter.SellerId.Value;
            query = query.Where(x => x.o.SellerId.Value == sid);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (!Enum.TryParse<OfferStatus>(filter.Status.Trim(), ignoreCase: true, out var parsed))
            {
                // неверный статус -> пустая страница
                var empty = Array.Empty<OfferReadModel>();
                var page0 = pagination.Page < 1 ? 1 : pagination.Page;
                var pageSize0 = pagination.PageSize is < 1 or > 200 ? 50 : pagination.PageSize;
                return new PagedResult<OfferReadModel>
                {
                    Items = empty,
                    TotalCount = 0,
                    Page = page0,
                    PageSize = pageSize0
                };
            }

            query = query.Where(x => x.o.Status == parsed);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchSku))
        {
            var sku = filter.SearchSku.Trim();
            query = query.Where(x => x.o.SellerSku != null && x.o.SellerSku == sku);
        }

        if (filter.MinPrice.HasValue)
        {
            var min = filter.MinPrice.Value;
            query = query.Where(x => x.o.Price != null && x.o.Price.Amount >= min);
        }

        if (filter.MaxPrice.HasValue)
        {
            var max = filter.MaxPrice.Value;
            query = query.Where(x => x.o.Price != null && x.o.Price.Amount <= max);
        }

        if (filter.InStock == true)
            query = query.Where(x => x.o.Stock > 0);

        // ---------- Sorting ----------
        query = query.OrderByDescending(x => x.o.CreatedAt);

        // ---------- Paging ----------
        var page = pagination.Page < 1 ? 1 : pagination.Page;
        var pageSize = pagination.PageSize is < 1 or > 200 ? 50 : pagination.PageSize;

        var total = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new OfferReadModel
            {
                Id = x.o.Id.Value,
                ProductId = x.o.ProductId.Value,
                ProductName = x.p.Name,
                SellerId = x.o.SellerId.Value,
                SellerName = x.s.Name,

                PriceAmount = x.o.Price != null ? x.o.Price.Amount : (decimal?)null,
                PriceCurrency = x.o.Price != null ? x.o.Price.Currency : null,
                OldPriceAmount = x.o.OldPrice != null ? x.o.OldPrice.Amount : (decimal?)null,

                Stock = x.o.Stock,
                SellerSku = x.o.SellerSku,
                DeliveryDays = x.o.DeliveryDays,
                MinOrderQuantity = x.o.MinOrderQuantity,

                Status = x.o.Status.ToString(),
                CreatedAt = x.o.CreatedAt,
                UpdatedAt = x.o.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        // total у тебя long, а TotalCount в DTO int -> лучше считать int
        var totalInt = await query.CountAsync(cancellationToken);

        return new PagedResult<OfferReadModel>
        {
            Items = items,
            TotalCount = totalInt,
            Page = page,
            PageSize = pageSize
        };
    }*/
}