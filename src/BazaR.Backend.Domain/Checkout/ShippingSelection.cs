using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.ShippingProfiles;

namespace BazaR.Backend.Domain.Checkouts;

public sealed class ShippingSelection
{
    private ShippingSelection() { }

    private ShippingSelection(
        ShippingMethodType methodType,
        string country,
        string region,
        string city,
        string? street,
        string? house,
        string? apartment,
        string? postalCode,
        string? pickupPointCode,
        string? pickupPointName,
        string? comment,
        Money cost)
    {
        MethodType = methodType;
        Country = country;
        Region = region;
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
        PostalCode = postalCode;
        PickupPointCode = pickupPointCode;
        PickupPointName = pickupPointName;
        Comment = comment;
        Cost = cost;
    }

    public ShippingMethodType MethodType { get; private set; }
    public string Country { get; private set; } = default!;
    public string Region { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string? Street { get; private set; }
    public string? House { get; private set; }
    public string? Apartment { get; private set; }
    public string? PostalCode { get; private set; }

    public string? PickupPointCode { get; private set; }
    public string? PickupPointName { get; private set; }

    public string? Comment { get; private set; }
    public Money Cost { get; private set; } = default!;

    public static Result<ShippingSelection> Create(
        ShippingMethodType methodType,
        string country,
        string region,
        string city,
        string? street,
        string? house,
        string? apartment,
        string? postalCode,
        string? pickupPointCode,
        string? pickupPointName,
        string? comment,
        Money cost)
    {
        if (methodType == ShippingMethodType.Unknown)
        {
            return Result<ShippingSelection>.Failure(
                new Error("Checkout.Shipping.Method.Required", "Shipping method is required."));
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            return Result<ShippingSelection>.Failure(
                new Error("Checkout.Shipping.Country.Required", "Country is required."));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return Result<ShippingSelection>.Failure(
                new Error("Checkout.Shipping.City.Required", "City is required."));
        }

        if (cost is null)
        {
            return Result<ShippingSelection>.Failure(
                new Error("Checkout.Shipping.Cost.Required", "Shipping cost is required."));
        }

        var needsPickupPoint = methodType is
            ShippingMethodType.NovaPoshtaWarehouse or
            ShippingMethodType.NovaPoshtaLocker or
            ShippingMethodType.UkrPoshtaBranch or
            ShippingMethodType.BazaRPickup;

        var needsStreetAddress = methodType is
            ShippingMethodType.NovaPoshtaCourier or
            ShippingMethodType.UkrPoshtaCourier or
            ShippingMethodType.BazaRCourier;

        if (needsPickupPoint && string.IsNullOrWhiteSpace(pickupPointCode))
        {
            return Result<ShippingSelection>.Failure(
                new Error(
                    "Checkout.Shipping.PickupPoint.Required",
                    "Pickup point is required for selected shipping method."));
        }

        if (needsStreetAddress)
        {
            if (string.IsNullOrWhiteSpace(street))
            {
                return Result<ShippingSelection>.Failure(
                    new Error("Checkout.Shipping.Street.Required", "Street is required for selected shipping method."));
            }

            if (string.IsNullOrWhiteSpace(house))
            {
                return Result<ShippingSelection>.Failure(
                    new Error("Checkout.Shipping.House.Required", "House is required for selected shipping method."));
            }
        }

        return Result<ShippingSelection>.Success(new ShippingSelection(
            methodType,
            country.Trim(),
            (region ?? string.Empty).Trim(),
            city.Trim(),
            Normalize(street),
            Normalize(house),
            Normalize(apartment),
            Normalize(postalCode),
            Normalize(pickupPointCode),
            Normalize(pickupPointName),
            Normalize(comment),
            cost));
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}