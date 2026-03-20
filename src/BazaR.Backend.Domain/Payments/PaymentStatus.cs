namespace BazaR.Backend.Domain.Payments;

public enum PaymentStatus
{
    Pending = 0,
    RequiresAction = 1,
    Authorized = 2,
    Paid = 3,
    Failed = 4,
    Cancelled = 5,
    PartiallyRefunded = 6,
    Refunded = 7
}