using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.PaymentProfiles;

public static class PaymentProfileErrors
{
    public static readonly Error SellerIdRequired =
        new("PaymentProfile.Seller.Required", "Seller is required.");

    public static readonly Error ArchivedProfileCannotBeModified =
        new("PaymentProfile.Archived.CannotModify", "Archived payment profile cannot be modified.");

    public static readonly Error ProfileMustHaveMethods =
        new("PaymentProfile.Methods.Required", "Payment profile must have methods.");

    public static readonly Error ProfileMustHaveEnabledMethods =
        new("PaymentProfile.EnabledMethods.Required", "Payment profile must have enabled methods.");

    public static readonly Error BankAccountRequired =
        new("PaymentProfile.BankAccount.Required", "Bank account is required for enabled payment methods.");

    public static readonly Error LiqPaySettingsRequired =
        new("PaymentProfile.LiqPay.Required", "LiqPay settings are required for enabled payment methods.");

    public static readonly Error MethodAlreadyExists =
        new("PaymentMethod.AlreadyExists", "Payment method already exists.");

    public static readonly Error MethodNotFound =
        new("PaymentMethod.NotFound", "Payment method was not found.");

    public static readonly Error MethodTypeRequired =
        new("PaymentMethod.Type.Required", "Payment method type is required.");

    public static readonly Error MinAmountInvalid =
        new("PaymentMethod.MinAmount.Invalid", "Min amount is invalid.");

    public static readonly Error MaxAmountInvalid =
        new("PaymentMethod.MaxAmount.Invalid", "Max amount is invalid.");

    public static readonly Error AmountRangeInvalid =
        new("PaymentMethod.Range.Invalid", "Min amount cannot exceed max amount.");

    public static readonly Error BankRecipientNameRequired =
        new("BankAccount.RecipientName.Required", "Recipient name is required.");

    public static readonly Error BankIbanRequired =
        new("BankAccount.Iban.Required", "IBAN is required.");

    public static readonly Error BankNameRequired =
        new("BankAccount.BankName.Required", "Bank name is required.");

    public static readonly Error TaxNumberRequired =
        new("BankAccount.TaxNumber.Required", "Tax number is required.");

    public static readonly Error LiqPayPublicKeyRequired =
        new("LiqPay.PublicKey.Required", "LiqPay public key is required.");

    public static readonly Error LiqPayPrivateKeyRequired =
        new("LiqPay.PrivateKey.Required", "LiqPay private key is required.");
}