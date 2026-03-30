namespace BazaR.Backend.Api.Contracts.Shippings;

public sealed record SetShippingSenderRequest(
    string Name,
    string Phone,
    string CountryCode,
    string? PickupPointCode,
    string? PickupPointName);