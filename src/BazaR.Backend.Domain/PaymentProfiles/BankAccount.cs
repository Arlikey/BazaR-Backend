using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.PaymentProfiles;

public sealed class BankAccount : ValueObject
{
    public string RecipientName { get; private set; } = default!;
    public string Iban { get; private set; } = default!;
    public string BankName { get; private set; } = default!;
    public string TaxNumber { get; private set; } = default!;
    public string? Swift { get; private set; }
    public string? PurposeTemplate { get; private set; }

    private BankAccount() { }

    private BankAccount(
        string recipientName,
        string iban,
        string bankName,
        string taxNumber,
        string? swift,
        string? purposeTemplate)
    {
        RecipientName = recipientName;
        Iban = iban;
        BankName = bankName;
        TaxNumber = taxNumber;
        Swift = swift;
        PurposeTemplate = purposeTemplate;
    }

    public static Result<BankAccount> Create(
        string recipientName,
        string iban,
        string bankName,
        string taxNumber,
        string? swift,
        string? purposeTemplate)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
            return Result<BankAccount>.Failure(PaymentProfileErrors.BankRecipientNameRequired);

        if (string.IsNullOrWhiteSpace(iban))
            return Result<BankAccount>.Failure(PaymentProfileErrors.BankIbanRequired);

        if (string.IsNullOrWhiteSpace(bankName))
            return Result<BankAccount>.Failure(PaymentProfileErrors.BankNameRequired);

        if (string.IsNullOrWhiteSpace(taxNumber))
            return Result<BankAccount>.Failure(PaymentProfileErrors.TaxNumberRequired);

        return Result<BankAccount>.Success(new BankAccount(
            recipientName.Trim(),
            iban.Trim().Replace(" ", "").ToUpperInvariant(),
            bankName.Trim(),
            taxNumber.Trim(),
            string.IsNullOrWhiteSpace(swift) ? null : swift.Trim().ToUpperInvariant(),
            string.IsNullOrWhiteSpace(purposeTemplate) ? null : purposeTemplate.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return RecipientName;
        yield return Iban;
        yield return BankName;
        yield return TaxNumber;
        yield return Swift;
        yield return PurposeTemplate;
    }
}