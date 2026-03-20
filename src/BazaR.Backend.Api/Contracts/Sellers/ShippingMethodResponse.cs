namespace BazaR.Backend.Api.Contracts.Sellers
{
    public sealed record ShippingMethodResponse(
     string MethodType,
     bool IsEnabled,
     decimal BaseFee,
     string Currency,
     decimal? FreeShippingFromAmount,
     bool AllowCashOnDelivery,
     bool RequiresCity,
     bool RequiresPickupPoint,
     bool RequiresStreetAddress,
     int? EstimatedDaysMin,
     int? EstimatedDaysMax,
     string? Title,
     string? Description);
}
