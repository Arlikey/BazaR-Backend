using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog;

public sealed class VendorCode : ValueObject
{
    private const int MaxLength = 64;
    public string Value { get; }

    private VendorCode(string value) => Value = value;

    public static Result<VendorCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<VendorCode>.Failure(ProductErrors.VendorCodeRequired);

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength)
            return Result<VendorCode>.Failure(ProductErrors.VendorCodeTooLong);

        return Result<VendorCode>.Success(new VendorCode(trimmed));
    }

    // Метод From для создания из уже валидных данных (например, из БД)
    public static VendorCode From(string value) => new(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}