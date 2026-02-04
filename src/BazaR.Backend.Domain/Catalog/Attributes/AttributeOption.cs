using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Attributes;

public sealed class AttributeOption
{
    private const int MaxValueLength = 200;

    public Guid Id { get; private set; }
    public string Value { get; private set; } = default!;
    public int SortOrder { get; private set; }

    private AttributeOption(Guid id, string value, int sortOrder)
    {
        Id = id;
        Value = value;
        SortOrder = sortOrder;
    }

    private AttributeOption() { } // EF

    internal static Result<AttributeOption> Create(string value, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<AttributeOption>.Failure(AttributeErrors.OptionValueRequired);

        var trimmed = value.Trim();
        if (trimmed.Length > MaxValueLength)
            return Result<AttributeOption>.Failure(AttributeErrors.OptionValueTooLong);

        if (sortOrder < 0)
            return Result<AttributeOption>.Failure(AttributeErrors.SortOrderCannotBeNegative);

        return Result<AttributeOption>.Success(
            new AttributeOption(Guid.NewGuid(), trimmed, sortOrder));
    }

    internal Result Rename(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure(AttributeErrors.OptionValueRequired);

        var trimmed = value.Trim();
        if (trimmed.Length > MaxValueLength)
            return Result.Failure(AttributeErrors.OptionValueTooLong);

        if (Value == trimmed)
            return Result.Success();

        Value = trimmed;
        return Result.Success();
    }

    internal Result SetSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
            return Result.Failure(AttributeErrors.SortOrderCannotBeNegative);

        if (SortOrder == sortOrder)
            return Result.Success();

        SortOrder = sortOrder;
        return Result.Success();
    }
}
