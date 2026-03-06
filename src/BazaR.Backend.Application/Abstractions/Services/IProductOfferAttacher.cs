using BazaR.Backend.Application.Abstractions.ReadModels;
namespace BazaR.Backend.Application.Abstractions.Services;

public interface IProductOfferAttacher
{
    Task<IReadOnlyList<ProductCardWithOfferDto>> AttachOffersAsync(
        IReadOnlyList<ProductCardDto> products,
        CancellationToken ct);
}