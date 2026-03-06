using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sellers;

public sealed record CountryCode
{
    public string Value { get; }

    private CountryCode(string value) => Value = value;

    public static Result<CountryCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<CountryCode>.Failure(new Error("Seller.CountryRequired", "Country code is required."));

        var v = value.Trim().ToUpperInvariant();
        if (v.Length != 2)
            return Result<CountryCode>.Failure(new Error("Seller.CountryInvalid", "Country code must be ISO alpha-2 (e.g., UA)."));

        return Result<CountryCode>.Success(new CountryCode(v));
    }
}
