namespace BazaR.Backend.Api.Contracts.Sellers
{
    public sealed record UpdateShippingMethodRequest(
     decimal BaseFee,
     string Currency,
     decimal? FreeShippingFromAmount,
     bool AllowCashOnDelivery,
     int? EstimatedDaysMin,
     int? EstimatedDaysMax,
     string? Title,
     string? Description);
}
