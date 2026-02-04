using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct)
    {
        return _db.Products.SingleOrDefaultAsync(p => p.Id == id, ct);
    }

    public void Add(Product product)
    {
        _db.Products.Add(product);
    }

    public void Remove(Product product)
    {
        _db.Products.Remove(product);
    }

    public async Task<bool> SlugExistsAsync(ProductSlug slug, ProductId? excludeProductId, CancellationToken ct)
    {
       
        var slugValue = slug.Value;

        if (excludeProductId.HasValue)
        {
            var excludeId = excludeProductId.Value.Value;
            return await _db.Products
                .FromSqlRaw(@"
                    SELECT * FROM products 
                    WHERE slug = {0} 
                    AND id != {1} 
                    LIMIT 1",
                    slugValue, excludeId)
                .AnyAsync(ct);
        }
        else
        {
            return await _db.Products
                .FromSqlRaw(@"
                    SELECT * FROM products 
                    WHERE slug = {0} 
                    LIMIT 1",
                    slugValue)
                .AnyAsync(ct);
        }
    }

    public async Task<bool> VendorCodeExistsAsync(VendorCode vendorCode, ProductId? excludeProductId, CancellationToken ct)
    {
        var vendorCodeValue = vendorCode.Value;

        if (excludeProductId.HasValue)
        {
            var excludeId = excludeProductId.Value.Value;
            return await _db.Products
                .FromSqlRaw(@"
                    SELECT * FROM products 
                    WHERE vendor_code = {0} 
                    AND id != {1} 
                    LIMIT 1",
                    vendorCodeValue, excludeId)
                .AnyAsync(ct);
        }
        else
        {
            return await _db.Products
                .FromSqlRaw(@"
                    SELECT * FROM products 
                    WHERE vendor_code = {0} 
                    LIMIT 1",
                    vendorCodeValue)
                .AnyAsync(ct);
        }
    }
}