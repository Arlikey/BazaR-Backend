using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Shipping;
using BazaR.Backend.Domain.ShippingProfiles;

namespace BazaR.Backend.Application.Checkouts;

public sealed class ShippingSelectionFactory : IShippingSelectionFactory
{
    public Result<ShippingSelection> Create(
        ShippingProfile profile,
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
        string? comment)
    {
        if (profile is null)
        {
            return Result<ShippingSelection>.Failure(
                new Error("ShippingProfile.Required", "Shipping profile is required."));
        }

        if (profile.Status != ShippingProfileStatus.Active)
        {
            return Result<ShippingSelection>.Failure(
                new Error("ShippingProfile.NotActive", "Shipping profile is not active."));
        }

        if (methodType == ShippingMethodType.Unknown)
        {
            return Result<ShippingSelection>.Failure(
                new Error("Checkout.Shipping.Method.Invalid", "Shipping method is invalid."));
        }

        var methodConfig = profile.FindEnabledMethod(methodType);
        if (methodConfig is null)
        {
            return Result<ShippingSelection>.Failure(
                new Error("ShippingProfile.Method.NotAvailable", "Selected shipping method is not available."));
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            return Result<ShippingSelection>.Failure(
                new Error("Checkout.Shipping.Country.Required", "Country is required."));
        }

        if (methodConfig.RequiresCity && string.IsNullOrWhiteSpace(city))
        {
            return Result<ShippingSelection>.Failure(
                new Error("Checkout.Shipping.City.Required", "City is required for selected shipping method."));
        }

        var hasStreetAddress =
            !string.IsNullOrWhiteSpace(street) &&
            !string.IsNullOrWhiteSpace(house);

        if (methodConfig.RequiresStreetAddress && !hasStreetAddress)
        {
            return Result<ShippingSelection>.Failure(
                new Error(
                    "Checkout.Shipping.Address.Required",
                    "Street and house are required for selected shipping method."));
        }

        if (methodConfig.RequiresPickupPoint && string.IsNullOrWhiteSpace(pickupPointCode))
        {
            return Result<ShippingSelection>.Failure(
                new Error(
                    "Checkout.Shipping.PickupPoint.Required",
                    "Pickup point is required for selected shipping method."));
        }

        var costResult = Money.Create(methodConfig.BaseFee, methodConfig.Currency);
        if (costResult.IsFailure)
        {
            return Result<ShippingSelection>.Failure(costResult.Error);
        }

        return ShippingSelection.Create(
            methodType,
            country,
            region,
            city,
            Normalize(street),
            Normalize(house),
            Normalize(apartment),
            Normalize(postalCode),
            Normalize(pickupPointCode),
            Normalize(pickupPointName),
            Normalize(comment),
            costResult.Value!);
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}