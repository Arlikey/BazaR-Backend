using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Shippings;

public sealed record ShippingDestination(
    string Country,
    string Region,
    string City,
    string? Street,
    string? House,
    string? Apartment,
    string? PostalCode,
    string? PickupPointCode,
    string? PickupPointName)
{
    public static Result<ShippingDestination> Create(
        string country,
        string region,
        string city,
        string? street,
        string? house,
        string? apartment,
        string? postalCode,
        string? pickupPointCode,
        string? pickupPointName)
    {
        if (string.IsNullOrWhiteSpace(country))
            return Result<ShippingDestination>.Failure(ShippingErrors.CountryRequired);

        if (string.IsNullOrWhiteSpace(city))
            return Result<ShippingDestination>.Failure(ShippingErrors.CityRequired);

        return Result<ShippingDestination>.Success(new ShippingDestination(
            country.Trim(),
            (region ?? string.Empty).Trim(),
            city.Trim(),
            string.IsNullOrWhiteSpace(street) ? null : street.Trim(),
            string.IsNullOrWhiteSpace(house) ? null : house.Trim(),
            string.IsNullOrWhiteSpace(apartment) ? null : apartment.Trim(),
            string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim(),
            string.IsNullOrWhiteSpace(pickupPointCode) ? null : pickupPointCode.Trim(),
            string.IsNullOrWhiteSpace(pickupPointName) ? null : pickupPointName.Trim()));
    }
}