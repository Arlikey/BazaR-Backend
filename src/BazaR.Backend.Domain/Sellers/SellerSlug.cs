using System.Text.RegularExpressions;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sellers;

public sealed record SellerSlug
{
    private const int MaxLength = 200;
    private static readonly Regex Rx = new("^[a-z0-9_\\-]+$", RegexOptions.Compiled);

    public string Value { get; }

    private SellerSlug(string value) => Value = value;

    public static Result<SellerSlug> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<SellerSlug>.Failure(new Error("Seller.SlugRequired", "Seller slug is required."));

        var v = value.Trim().ToLowerInvariant();
        if (v.Length > MaxLength)
            return Result<SellerSlug>.Failure(new Error("Seller.SlugTooLong", "Seller slug is too long."));

        if (!Rx.IsMatch(v))
            return Result<SellerSlug>.Failure(new Error("Seller.SlugInvalid", "Slug may contain a-z, 0-9, '_' or '-' only."));

        return Result<SellerSlug>.Success(new SellerSlug(v));
    }
}
