using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface IProductReadRepository
{
    //sk<IReadOnlyList<ProductListItemDto>> ListAsync(CancellationToken ct);
    Task<IReadOnlyList<ProductCardDto>> ListByCategoryAsync(
    CategoryId categoryId,
    ProductStatus? status,
    CancellationToken ct);
    Task<PagedResult<ProductCardDto>> SearchAsync(
    ProductSearchFilter filter,
    Pagination pagination,
    CancellationToken ct);

    Task<IReadOnlyList<ProductCardDto>> ListBySellerAsync(
    SellerId sellerId,
    int limit,
    CancellationToken ct);

    Task<ProductDetailsDto?> GetByIdAsync(ProductId id, CancellationToken ct);
    Task<ProductDetailsDto?> GetByIdForOwnerAsync(
       ProductId productId,
       SellerId ownerSellerId,
       CancellationToken ct);
}
