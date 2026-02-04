using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Products;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct);

    void Add(Product product);
    void Remove(Product product);

    /// <summary>
    /// Проверка уникальности Slug (обычно + уникальный индекс в БД).
    /// excludeProductId нужен для обновления продукта.
    /// </summary>
    Task<bool> SlugExistsAsync(ProductSlug slug, ProductId? excludeProductId, CancellationToken ct);

    /// <summary>
    /// Проверка уникальности VendorCode (обычно + уникальный индекс в БД).
    /// excludeProductId нужен для обновления продукта.
    /// </summary>
    Task<bool> VendorCodeExistsAsync(VendorCode vendorCode, ProductId? excludeProductId, CancellationToken ct);
}
