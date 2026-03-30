using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Shippings;

public sealed record ShippingSender
{
    public string Name { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string CountryCode { get; private set; } = default!;

    public string? PickupPointCode { get; private set; }
    public string? PickupPointName { get; private set; }

    private ShippingSender() { }

    private ShippingSender(
        string name,
        string phone,
        string countryCode,
        string? pickupPointCode,
        string? pickupPointName)
    {
        Name = name;
        Phone = phone;
        CountryCode = countryCode;
        PickupPointCode = pickupPointCode;
        PickupPointName = pickupPointName;
    }

    public static Result<ShippingSender> Create(
        string name,
        string phone,
        string countryCode,
        string? pickupPointCode,
        string? pickupPointName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<ShippingSender>.Failure(new Error(
                "ShippingSender.Name.Required",
                "Sender name is required."));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            return Result<ShippingSender>.Failure(new Error(
                "ShippingSender.Phone.Required",
                "Sender phone is required."));
        }

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return Result<ShippingSender>.Failure(new Error(
                "ShippingSender.CountryCode.Required",
                "Sender country code is required."));
        }

        return Result<ShippingSender>.Success(new ShippingSender(
            name.Trim(),
            phone.Trim(),
            countryCode.Trim().ToUpperInvariant(),
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
                "ShippingSender.PickupPoint.Required",
                "Sender pickup point code is required."));
        }

        PickupPointCode = pickupPointCode.Trim();
        PickupPointName = Normalize(pickupPointName);

        return Result.Success();
    }

    public Result UpdateContactInfo(
        string name,
        string phone,
        string countryCode)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(new Error(
                "ShippingSender.Name.Required",
                "Sender name is required."));
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            return Result.Failure(new Error(
                "ShippingSender.Phone.Required",
                "Sender phone is required."));
        }

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return Result.Failure(new Error(
                "ShippingSender.CountryCode.Required",
                "Sender country code is required."));
        }

        Name = name.Trim();
        Phone = phone.Trim();
        CountryCode = countryCode.Trim().ToUpperInvariant();

        return Result.Success();
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}