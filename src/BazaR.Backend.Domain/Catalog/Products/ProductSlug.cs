using System.Text;
using System.Text.RegularExpressions;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog;

public sealed class ProductSlug : ValueObject
{
    private const int MaxLength = 200;

    public string Value { get; }

    private ProductSlug(string value)
    {
        Value = value;
    }

    public static Result<ProductSlug> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<ProductSlug>.Failure(ProductErrors.SlugRequired);

        var normalized = Normalize(value);

        if (normalized.Length == 0)
            return Result<ProductSlug>.Failure(ProductErrors.SlugInvalid);

        if (normalized.Length > MaxLength)
            return Result<ProductSlug>.Failure(ProductErrors.SlugTooLong);

        return Result<ProductSlug>.Success(new ProductSlug(normalized));
    }

    // Метод From для создания из уже валидных данных (например, из БД)
    public static ProductSlug From(string value) => new(value);

    private static string Normalize(string input)
    {
        input = input.Trim().ToLowerInvariant();

        var sb = new StringBuilder(input.Length);
        foreach (var ch in input)
        {
            if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9'))
                sb.Append(ch);
            else
                sb.Append('-');
        }

        var result = sb.ToString();
        result = Regex.Replace(result, "-{2,}", "-").Trim('-');

        return result;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}