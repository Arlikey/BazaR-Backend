using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sales.Offers.Commands.UpsertMyOffer;

public sealed record UpsertMyOfferCommand(
    Guid ProductId,
    decimal? PriceAmount,
    string? PriceCurrency,
    int Stock
) : IRequest<Result>;

