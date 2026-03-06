using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface ISellerRepository
{
   
    Task<Seller?> GetByIdAsync(SellerId id, CancellationToken ct = default);

    void Add(Seller seller);
    void Update(Seller seller);
    void Remove(Seller seller);

   
    Task<bool> ExistsAsync(SellerId id, CancellationToken ct = default);

    Task<Seller?> GetByOwnerUserIdAsync(Guid ownerUserId, CancellationToken ct = default);

    Task<Seller?> GetBySlugAsync(string slug, CancellationToken ct = default);

    Task<bool> SlugExistsAsync(SellerSlug slug, SellerId? excludeSellerId = null, CancellationToken ct = default);

    Task<bool> TaxNumberExistsAsync(string taxNumber, SellerId? excludeSellerId = null, CancellationToken ct = default);
}
