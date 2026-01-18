namespace BazaR.Backend.Domain.Orders;

public enum OrderStatus
{
    Pending = 1,   // created, waiting for payment
    Paid = 2,
    Cancelled = 3
}
