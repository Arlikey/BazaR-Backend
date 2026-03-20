using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Orders;

public static class OrderErrors
{
    public static readonly Error EmptyOrder =
        new("Order.Empty", "Order cannot be empty.", ErrorType.Validation);

    public static readonly Error InvalidQuantity =
        new("Order.InvalidQuantity", "Order item quantity must be greater than zero.", ErrorType.Validation);

    public static readonly Error CurrencyMismatch =
        new("Order.CurrencyMismatch", "All order items must have the same currency.", ErrorType.Validation);

    public static readonly Error AlreadyPaid =
        new("Order.AlreadyPaid", "Order is already paid.", ErrorType.Conflict);

    public static readonly Error AlreadyCancelled =
        new("Order.AlreadyCancelled", "Order is already cancelled.", ErrorType.Conflict);

    public static readonly Error CannotPayCancelled =
        new("Order.CannotPayCancelled", "Cancelled order cannot be paid.", ErrorType.Conflict);

    public static readonly Error CannotCancelPaid =
        new("Order.CannotCancelPaid", "Paid order cannot be cancelled.", ErrorType.Conflict);

    public static readonly Error OnlyPendingCanAwaitPayment =
        new("Order.OnlyPendingCanAwaitPayment", "Only pending orders can be moved to awaiting payment.", ErrorType.Conflict);

    public static readonly Error OnlyPendingOrAwaitingCanBePaid =
        new("Order.OnlyPendingOrAwaitingCanBePaid", "Only pending or awaiting payment orders can be paid.", ErrorType.Conflict);

    public static readonly Error OnlyPaidCanBeProcessing =
        new("Order.OnlyPaidCanBeProcessing", "Only paid orders can be moved to processing.", ErrorType.Conflict);

    public static readonly Error OnlyProcessingCanBeShipped =
        new("Order.OnlyProcessingCanBeShipped", "Only processing orders can be shipped.", ErrorType.Conflict);

    public static readonly Error OnlyShippedCanBeDelivered =
        new("Order.OnlyShippedCanBeDelivered", "Only shipped orders can be delivered.", ErrorType.Conflict);

    public static readonly Error OnlyDeliveredCanBeCompleted =
        new("Order.OnlyDeliveredCanBeCompleted", "Only delivered orders can be completed.", ErrorType.Conflict);

    public static readonly Error CannotCancelAfterShipping =
        new("Order.CannotCancelAfterShipping", "Shipped, delivered or completed orders cannot be cancelled.", ErrorType.Conflict);

    public static readonly Error AddressChangeNotAllowed =
        new("Order.AddressChangeNotAllowed", "Cannot change delivery address after payment or processing has started.", ErrorType.Conflict);

    public static readonly Error ProductNameRequired =
        new("Order.ProductNameRequired", "Product name is required.", ErrorType.Validation);

    public static readonly Error CommentChangeNotAllowed =
    new("Order.CommentChangeNotAllowed", "Customer comment can only be changed before payment.", ErrorType.Conflict);

    public static readonly Error CancellationReasonRequired =
        new("Order.CancellationReasonRequired", "Cancellation reason is required.", ErrorType.Validation);

    public static readonly Error PartialCancellationNotAllowed =
        new("Order.PartialCancellationNotAllowed", "Partial cancellation is not allowed for the current order status.", ErrorType.Conflict);

    public static readonly Error ItemNotFound =
        new("Order.ItemNotFound", "Order item was not found.", ErrorType.NotFound);

    public static readonly Error InvalidCancellationQuantity =
        new("Order.InvalidCancellationQuantity", "Cancellation quantity must be greater than zero.", ErrorType.Validation);

    public static readonly Error CancellationQuantityTooLarge =
        new("Order.CancellationQuantityTooLarge", "Cancellation quantity exceeds the active quantity.", ErrorType.Validation);

    public static readonly Error AddressInvalid =
    new("Order.AddressInvalid", "Delivery address is invalid.", ErrorType.Validation);

    public static readonly Error DeliveryChangeNotAllowed = new("Order.DeliveryChangeNotAllowed", "Cannot change delivery in current status");
}