using System.Collections.Generic;
using BazaR.Backend.Domain.Catalog;

namespace BazaR.Backend.Domain.Common;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = decimal.Round(amount, 2);
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency = "UAH")
    {
        if (amount < 0)
            return Result<Money>.Failure(ProductErrors.PriceMustBePositive);

        if (string.IsNullOrWhiteSpace(currency))
            return Result<Money>.Failure(ProductErrors.CurrencyRequired);

        var normalized = currency.Trim().ToUpperInvariant();

        return Result<Money>.Success(new Money(amount, normalized));
    }

    public static Money Zero(string currency = "UAH")
    {
        return new Money(0m, currency.ToUpperInvariant());
    }

    public static Money operator +(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static Money operator *(Money money, int multiplier)
    {
        if (multiplier < 0)
            throw new InvalidOperationException("Multiplier must be >= 0");

        return new Money(money.Amount * multiplier, money.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        if (multiplier < 0)
            throw new InvalidOperationException("Multiplier must be >= 0");

        return new Money(money.Amount * multiplier, money.Currency);
    }

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Currency mismatch.");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount} {Currency}";
}