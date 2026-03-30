using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Shippings;

public sealed record ShippingDestination
{
    public string Country { get; private set; } = default!;
    public string Region { get; private set; } = string.Empty;
    public string City { get; private set; } = default!;
    public string? Street { get; private set; }
    public string? House { get; private set; }
    public string? Apartment { get; private set; }
    public string? PostalCode { get; private set; }

    public string? PickupPointCode { get; private set; }
    public string? PickupPointName { get; private set; }

    private ShippingDestination() { }

    private ShippingDestination(
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
        Country = country;
        Region = region;
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
        PostalCode = postalCode;
        PickupPointCode = pickupPointCode;
        PickupPointName = pickupPointName;
    }

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
            Normalize(region) ?? string.Empty,
            city.Trim(),
            Normalize(street),
            Normalize(house),
            Normalize(apartment),
            Normalize(postalCode),
            Normalize(pickupPointCode),
            Normalize(pickupPointName)));
    }

    public Result SetPickupPoint(
        string pickupPointCode,
        string? pickupPointName)
    {
        if (string.IsNullOrWhiteSpace(pickupPointCode))
        {
            return Result.Failure(new Error(
                "ShippingDestination.PickupPoint.Required",
                "Pickup point code is required."));
        }

        PickupPointCode = pickupPointCode.Trim();
        PickupPointName = Normalize(pickupPointName);

        return Result.Success();
    }

    public Result UpdateAddress(
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
            return Result.Failure(ShippingErrors.CountryRequired);

        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure(ShippingErrors.CityRequired);

        Country = country.Trim();
        Region = Normalize(region) ?? string.Empty;
        City = city.Trim();
        Street = Normalize(street);
        House = Normalize(house);
        Apartment = Normalize(apartment);
        PostalCode = Normalize(postalCode);
        PickupPointCode = Normalize(pickupPointCode);
        PickupPointName = Normalize(pickupPointName);

        return Result.Success();
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}