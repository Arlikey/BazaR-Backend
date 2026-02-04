using System.Collections.Generic;
using BazaR.Backend.Domain.Catalog;

namespace BazaR.Backend.Domain.Common;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency = "UAH")
    {
        if (amount <= 0)
            return Result<Money>.Failure(ProductErrors.PriceMustBePositive);

        if (string.IsNullOrWhiteSpace(currency))
            return Result<Money>.Failure(ProductErrors.CurrencyRequired);

        var normalized = currency.Trim().ToUpperInvariant();
        return Result<Money>.Success(new Money(decimal.Round(amount, 2), normalized));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount} {Currency}";
}
