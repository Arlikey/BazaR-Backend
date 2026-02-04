using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog;

public static class MoneyErrors
{
    public static readonly Error AmountCannotBeNegative =
        new("Money.AmountCannotBeNegative", "Money amount cannot be negative.");

    public static readonly Error CurrencyRequired =
        new("Money.CurrencyRequired", "Currency is required.");

    public static readonly Error CurrencyInvalid =
        new("Money.CurrencyInvalid", "Currency must be a 3-letter ISO code (e.g. UAH, USD, EUR).");
}
