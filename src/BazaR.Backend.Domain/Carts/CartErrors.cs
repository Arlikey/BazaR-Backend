using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Carts;

public static class CartErrors
{
    public static readonly Error UserIdRequired =
        new("Cart.UserIdRequired", "UserId is required.");

    public static readonly Error CannotModifyNonActive =
        new("Cart.CannotModifyNonActive", "Cart is not active and cannot be modified.");

    public static readonly Error EmptyCart =
        new("Cart.EmptyCart", "Cart is empty.");

    public static readonly Error OfferRequired =
        new("Cart.OfferRequired", "OfferId is required.");

    public static readonly Error QuantityMustBePositive =
        new("Cart.QuantityMustBePositive", "Quantity must be at least 1.");

    public static readonly Error MaxQuantityExceeded =
        new("Cart.MaxQuantityExceeded", "Maximum quantity per item exceeded.");

    public static readonly Error ItemNotFound =
        new("Cart.ItemNotFound", "Cart item not found.");

    public static Error CurrencyMismatch(string cartCurrency, string itemCurrency)
        => new("Cart.CurrencyMismatch", $"Cart currency '{cartCurrency}' does not match item currency '{itemCurrency}'.");
}