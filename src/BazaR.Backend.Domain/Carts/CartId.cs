using System;
using System.Collections.Generic;
using System.Linq;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales; // OfferId
using BazaR.Backend.Domain.Users;
using BazaR.Backend.Domain.Carts.Events;

namespace BazaR.Backend.Domain.Carts;

// ===========================
// Value Objects / IDs
// ===========================

public readonly record struct CartId(Guid Value)
{
    public static CartId New() => new(Guid.NewGuid());
}

public sealed record MoneySnapshot(decimal Amount, string Currency)
{
    public static Result<MoneySnapshot> Create(decimal amount, string currency)
    {
        if (amount < 0)
            return Result<MoneySnapshot>.Failure(new Error("Money.AmountNegative", "Amount cannot be negative."));

        if (string.IsNullOrWhiteSpace(currency))
            return Result<MoneySnapshot>.Failure(new Error("Money.CurrencyRequired", "Currency is required."));

        var cur = currency.Trim().ToUpperInvariant();
        if (cur.Length != 3)
            return Result<MoneySnapshot>.Failure(new Error("Money.CurrencyInvalid", "Currency must be 3 letters."));

        return Result<MoneySnapshot>.Success(new MoneySnapshot(amount, cur));
    }
}