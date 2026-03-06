/*using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sales.Offers.Queries.GetMyOfferByProductId;

public sealed record GetMyOfferByProductIdQuery(Guid ProductId)
    : IRequest<Result<MyOfferDto>>;

public sealed record MyOfferDto(
    Guid ProductId,
    Guid SellerId,
    decimal? PriceAmount,
    string? PriceCurrency,
    int Stock,
    string Status
);
*/