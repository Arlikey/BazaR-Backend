using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sellers;

public sealed record SellerShippingSettings
{
    public string SenderName { get; private set; } = default!;
    public string SenderPhone { get; private set; } = default!;
    public string? SenderEmail { get; private set; }
    public string CountryCode { get; private set; } = default!;

    public string? NovaPostDivisionId { get; private set; }
    public string? NovaPostDivisionName { get; private set; }

    private SellerShippingSettings() { }

    private SellerShippingSettings(
        string senderName,
        string senderPhone,
        string? senderEmail,
        string countryCode,
        string? novaPostDivisionId,
        string? novaPostDivisionName)
    {
        SenderName = senderName;
        SenderPhone = senderPhone;
        SenderEmail = senderEmail;
        CountryCode = countryCode;
        NovaPostDivisionId = novaPostDivisionId;
        NovaPostDivisionName = novaPostDivisionName;
    }

    public static Result<SellerShippingSettings> Create(
        string senderName,
        string senderPhone,
        string? senderEmail,
        string countryCode,
        string? novaPostDivisionId,
        string? novaPostDivisionName)
    {
        if (string.IsNullOrWhiteSpace(senderName))
        {
            return Result<SellerShippingSettings>.Failure(new Error(
                "SellerShippingSettings.SenderName.Required",
                "Sender name is required."));
        }

        if (string.IsNullOrWhiteSpace(senderPhone))
        {
            return Result<SellerShippingSettings>.Failure(new Error(
                "SellerShippingSettings.SenderPhone.Required",
                "Sender phone is required."));
        }

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return Result<SellerShippingSettings>.Failure(new Error(
                "SellerShippingSettings.CountryCode.Required",
                "Country code is required."));
        }

        return Result<SellerShippingSettings>.Success(new SellerShippingSettings(
            senderName.Trim(),
            senderPhone.Trim(),
            Normalize(senderEmail),
            countryCode.Trim().ToUpperInvariant(),
            Normalize(novaPostDivisionId),
            Normalize(novaPostDivisionName)));
    }

    public Result Update(
        string senderName,
        string senderPhone,
        string? senderEmail,
        string countryCode,
        string? novaPostDivisionId,
        string? novaPostDivisionName)
    {
        if (string.IsNullOrWhiteSpace(senderName))
        {
            return Result.Failure(new Error(
                "SellerShippingSettings.SenderName.Required",
                "Sender name is required."));
        }

        if (string.IsNullOrWhiteSpace(senderPhone))
        {
            return Result.Failure(new Error(
                "SellerShippingSettings.SenderPhone.Required",
                "Sender phone is required."));
        }

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return Result.Failure(new Error(
                "SellerShippingSettings.CountryCode.Required",
                "Country code is required."));
        }

        SenderName = senderName.Trim();
        SenderPhone = senderPhone.Trim();
        SenderEmail = Normalize(senderEmail);
        CountryCode = countryCode.Trim().ToUpperInvariant();
        NovaPostDivisionId = Normalize(novaPostDivisionId);
        NovaPostDivisionName = Normalize(novaPostDivisionName);

        return Result.Success();
    }

    public Result SetNovaPostDivision(
        string divisionId,
        string? divisionName)
    {
        if (string.IsNullOrWhiteSpace(divisionId))
        {
            return Result.Failure(new Error(
                "SellerShippingSettings.Division.Required",
                "Nova Post division id is required."));
        }

        NovaPostDivisionId = divisionId.Trim();
        NovaPostDivisionName = Normalize(divisionName);

        return Result.Success();
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}