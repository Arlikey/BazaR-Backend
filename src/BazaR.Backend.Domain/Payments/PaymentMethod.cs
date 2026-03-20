namespace BazaR.Backend.Domain.Payments;

public enum PaymentMethod
{
    Unknown = 0,
    Card = 1,
    ApplePay = 2,
    GooglePay = 3,
    CashOnDelivery = 4,
    BankTransfer = 5
}