using BazaR.Backend.Application.Catalog.Offers.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface IOfferReadRepository
{
    Task<OfferReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    //Task<IReadOnlyList<OfferReadModel>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    //Task<IReadOnlyList<OfferReadModel>> GetBySellerAsync(Guid sellerId, CancellationToken cancellationToken = default);
    //Task<PagedResult<OfferReadModel>> SearchAsync(OfferFilter filter, Pagination pagination, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OfferCardDto>> GetByProductCardIdsAsync(
    IReadOnlyCollection<Guid> productIds,
    CancellationToken ct);
 

    Task<OfferDetailsDto?> GetByProductIdAsync(
        ProductId productId,
        CancellationToken ct);
}