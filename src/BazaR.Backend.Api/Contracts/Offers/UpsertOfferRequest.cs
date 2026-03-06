namespace BazaR.Backend.Api.Contracts.Seller.Offers;

public sealed record UpsertOfferRequest(
    decimal? PriceAmount,
    string? PriceCurrency,
    int Stock
);



public sealed record CreateOfferRequest(
    Guid ProductId,
    decimal PriceAmount,
    string PriceCurrency = "UAH",
    int Stock = 0,
    decimal? OldPriceAmount = null,
    string? OldPriceCurrency = null,
    string? SellerSku = null,
    int? DeliveryDays = null,
    int MinOrderQuantity = 1,
    bool Activate = true
);