namespace BazaR.Backend.Domain.Shippings;

public enum ShippingStatus
{
    AwaitingSender = 1,
    ReadyToDispatch = 2,
    Dispatched = 3,
    ReadyForPickup = 4,
    Delivered = 5,
    Cancelled = 6
}