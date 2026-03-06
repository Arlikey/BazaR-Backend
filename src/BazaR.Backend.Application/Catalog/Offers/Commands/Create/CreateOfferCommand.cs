using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Create;

public sealed record CreateOfferCommand(
    Guid ProductId,

    // главное
    decimal PriceAmount,
    string PriceCurrency = "UAH",
    int Stock = 0,

    // скидка (опционально)
    decimal? OldPriceAmount = null,
    string? OldPriceCurrency = null,

    // условия (опционально)
    string? SellerSku = null,
    int? DeliveryDays = null,
    int MinOrderQuantity = 1,

    // опционально: сразу активировать
    bool Activate = true
) : IRequest<Result<Guid>>;