using BazaR.Backend.Domain.Catalog.Products;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface IProductAttributesReadService
{
    Task<ProductAttributesViewDto?> GetAttributesViewAsync(ProductId productId, CancellationToken ct);
}
