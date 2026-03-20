using BazaR.Backend.Domain.ShippingProfiles;

namespace BazaR.Backend.Api.Contracts.Sellers
{
    public sealed record AddShippingMethodRequest(
    ShippingMethodType MethodType,
    decimal BaseFee,
    string Currency,
    decimal? FreeShippingFromAmount,
    bool AllowCashOnDelivery,
    int? EstimatedDaysMin,
    int? EstimatedDaysMax,
    string? Title,
    string? Description);
}
