namespace BazaR.Backend.Api.Contracts.Sellers
{
    public sealed record SetBankAccountRequest(
    string RecipientName,
    string Iban,
    string BankName,
    string TaxNumber,
    string? Swift,
    string? PurposeTemplate);
}
