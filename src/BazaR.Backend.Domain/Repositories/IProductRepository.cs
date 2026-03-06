using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct);

    
    Task<Product?> GetByIdForOwnerAsync(ProductId id, SellerId ownerSellerId, CancellationToken ct);

    
    Task<bool> SlugExistsAsync(ProductSlug slug, ProductId? excludeId, CancellationToken ct);
    Task<bool> VendorCodeExistsAsync(SellerId ownerSellerId, VendorCode vendorCode, ProductId? excludeId, CancellationToken ct);
    Task<bool> BarcodeExistsAsync(ProductBarcode barcode, ProductId? excludeId, CancellationToken ct);

    Task<bool> SlugExistsForSellerAsync(
       SellerId ownerSellerId,
       ProductSlug slug,
       ProductId? excludeProductId,
       CancellationToken ct = default);

    Task<bool> VendorCodeExistsForSellerAsync(
        SellerId ownerSellerId,
        VendorCode vendorCode,
        ProductId? excludeProductId,
        CancellationToken ct = default);

    void Add(Product product);
    void Update(Product product);
}
