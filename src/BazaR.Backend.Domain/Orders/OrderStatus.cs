namespace BazaR.Backend.Domain.Orders;

public enum OrderStatus
{
    Pending = 1,
    AwaitingPayment = 2,
    Paid = 3,
    Processing = 4,
    Shipped = 5,
    Delivered = 6,
    Completed = 7,
    Cancelled = 8
}