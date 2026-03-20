namespace BazaR.Backend.Api.Contracts.Customer
{
    public sealed record SetCheckoutLineRecipientRequest(
    Guid LineId,
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    bool IsCustomerRecipient);
}
