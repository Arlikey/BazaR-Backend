using BazaR.Backend.Domain.Common;
using System.Text.RegularExpressions;

namespace BazaR.Backend.Domain.Categories;

public sealed record CategorySlug
{
    private const int MaxLength = 200;

    public string Value { get; }

    private CategorySlug(string value)
    {
        Value = value;
    }

    public static Result<CategorySlug> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<CategorySlug>.Failure(CategoryErrors.SlugRequired);

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
            return Result<CategorySlug>.Failure(CategoryErrors.SlugTooLong);

        // простая валидация: только a-z, 0-9, -
        if (!Regex.IsMatch(normalized, "^[a-z0-9-]+$"))
            return Result<CategorySlug>.Failure(CategoryErrors.InvalidSlugFormat);

        return Result<CategorySlug>.Success(new CategorySlug(normalized));
    }

    public override string ToString() => Value;
}