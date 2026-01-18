using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Carts;

public static class CartErrors
{
    public static readonly Error InvalidQuantity =
        new("cart.quantity.invalid", "Quantity must be greater than zero.", ErrorType.Validation);

    public static readonly Error ItemNotFound =
        new("cart.item.notFound", "Cart item not found.", ErrorType.NotFound);

    public static readonly Error EmptyCart =
        new("cart.empty", "Cart is empty.", ErrorType.Validation);
}