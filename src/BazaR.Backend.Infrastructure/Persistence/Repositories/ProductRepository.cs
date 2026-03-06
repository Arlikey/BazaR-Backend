using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) => _db = db;

    public Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct)
        => _db.Products
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Product?> GetByIdForOwnerAsync(ProductId id, SellerId ownerSellerId, CancellationToken ct)
        => _db.Products
            .FirstOrDefaultAsync(x => x.Id == id && x.OwnerSellerId == ownerSellerId, ct);

    public Task<bool> SlugExistsAsync(ProductSlug slug, ProductId? excludeId, CancellationToken ct)
        => _db.Products.AnyAsync(x =>
            x.Slug != null &&
            x.Slug == slug &&
            (excludeId == null || x.Id != excludeId), ct);

    public Task<bool> VendorCodeExistsAsync(SellerId ownerSellerId, VendorCode vendorCode, ProductId? excludeId, CancellationToken ct)
        => _db.Products.AnyAsync(x =>
            x.OwnerSellerId == ownerSellerId &&
            x.VendorCode != null &&
            x.VendorCode == vendorCode &&
            (excludeId == null || x.Id != excludeId), ct);

    public Task<bool> BarcodeExistsAsync(ProductBarcode barcode, ProductId? excludeId, CancellationToken ct)
        => _db.Products.AnyAsync(x =>
            x.Barcode != null &&
            x.Barcode == barcode &&
            (excludeId == null || x.Id != excludeId), ct);


    public Task<bool> SlugExistsForSellerAsync(
    SellerId ownerSellerId,
    ProductSlug slug,
    ProductId? excludeProductId,
    CancellationToken ct = default)
    {
        var query = _db.Products.AsNoTracking()
            .Where(p => p.OwnerSellerId == ownerSellerId)
            .Where(p => p.Slug != null && p.Slug == slug);

        if (excludeProductId is not null)
            query = query.Where(p => p.Id != excludeProductId);

        return query.AnyAsync(ct);
    }



    public Task<bool> VendorCodeExistsForSellerAsync(
     SellerId ownerSellerId,
     VendorCode vendorCode,
     ProductId? excludeProductId,
     CancellationToken ct = default)
    {
        var query = _db.Products.AsNoTracking()
            .Where(p => p.OwnerSellerId == ownerSellerId)
            .Where(p => p.VendorCode != null && p.VendorCode == vendorCode);

        if (excludeProductId is not null)
            query = query.Where(p => p.Id != excludeProductId);

        return query.AnyAsync(ct);
    }




    public void Add(Product product) => _db.Products.Add(product);
    public void Update(Product product) => _db.Products.Update(product);
}
