using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Orders;

public static class OrderErrors
{
    public static readonly Error EmptyOrder =
        new("order.empty", "Order must contain at least one item.", ErrorType.Validation);

    public static readonly Error InvalidQuantity =
        new("order.quantity.invalid", "Quantity must be greater than zero.", ErrorType.Validation);

    public static readonly Error CurrencyMismatch =
        new("order.currency.mismatch", "All order items must have the same currency.", ErrorType.Validation);

    public static readonly Error CannotPayCancelled =
        new("order.pay.cancelled", "Cannot pay a cancelled order.", ErrorType.Conflict);

    public static readonly Error AlreadyPaid =
        new("order.pay.alreadyPaid", "Order is already paid.", ErrorType.Conflict);

    public static readonly Error CannotCancelPaid =
        new("order.cancel.paid", "Cannot cancel a paid order.", ErrorType.Conflict);

    public static readonly Error AlreadyCancelled =
        new("order.cancel.alreadyCancelled", "Order is already cancelled.", ErrorType.Conflict);

    public static readonly Error AddressInvalid =
        new("order.address.invalid", "Delivery address is invalid.", ErrorType.Validation);
}
