namespace BazaR.Backend.Domain.PaymentProfiles;

public enum PaymentMethodType
{
    Unknown = 0,
    CashOnDelivery = 1,         // Оплата при получении
    LiqPayCheckout = 2,         // Оплатить сейчас
    BankTransferLegal = 3,      // Безнал для юрлиц
    BankTransferIndividual = 4, // Безнал для физлиц
    PrivatPay = 5,              // PrivatPay
    Installments = 6            // Оплата частями / кредит
}