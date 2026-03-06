namespace BazaR.Backend.Api.Contracts.Seller.Offers;

public sealed record MyOfferResponse(
    Guid ProductId,
    Guid SellerId,
    decimal? PriceAmount,
    string? PriceCurrency,
    int Stock,
    string Status
);

public sealed record SetOfferPriceRequest(
    decimal Amount,
    string Currency = "UAH"
);


public sealed record SetOfferOldPriceRequest(
    decimal Amount,
    string Currency = "UAH"
);


public sealed record SetOfferStockRequest(int Stock);

public sealed record ChangeOfferStockRequest(int Amount);

public sealed record SetOfferSkuRequest(string? SellerSku);

public sealed record SetOfferDeliveryDaysRequest(int? DeliveryDays);

public sealed record SetOfferMinOrderQuantityRequest(int MinOrderQuantity);


