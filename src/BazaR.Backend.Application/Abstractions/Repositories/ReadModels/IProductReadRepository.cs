using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface IProductReadRepository
{
    Task<IReadOnlyList<ProductListItemDto>> ListAsync(CancellationToken ct);
    Task<IReadOnlyList<ProductListItemDto>> ListByCategoryAsync(CategoryId categoryId, CancellationToken ct);
    Task<IReadOnlyList<ProductListItemDto>> SearchAsync(string term, int limit, CancellationToken ct); // пригодится для админки
    Task<ProductDetailsDto?> GetByIdAsync(ProductId id, CancellationToken ct);
}
