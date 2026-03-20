namespace BazaR.Backend.Domain.Shippings;

public enum ShippingStatus
{
    Pending = 0,
    Preparing = 1,
    ReadyToShip = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Returned = 6
}