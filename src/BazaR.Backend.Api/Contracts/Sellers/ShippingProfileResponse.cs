namespace BazaR.Backend.Api.Contracts.Sellers
{
    public sealed record ShippingProfileResponse(
     Guid Id,
     Guid SellerId,
     string Status,
     DateTimeOffset CreatedAtUtc,
     DateTimeOffset UpdatedAtUtc,
     IReadOnlyCollection<ShippingMethodResponse> Methods);
}
