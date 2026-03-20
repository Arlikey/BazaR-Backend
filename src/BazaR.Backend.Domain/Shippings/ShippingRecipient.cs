using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Shippings;

public sealed record ShippingRecipient(
    string FirstName,
    string LastName,
    string Phone,
    string? Email)
{
    public string FullName => $"{FirstName} {LastName}".Trim();

    public static Result<ShippingRecipient> Create(
        string firstName,
        string lastName,
        string phone,
        string? email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<ShippingRecipient>.Failure(ShippingErrors.RecipientFirstNameRequired);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<ShippingRecipient>.Failure(ShippingErrors.RecipientLastNameRequired);

        if (string.IsNullOrWhiteSpace(phone))
            return Result<ShippingRecipient>.Failure(ShippingErrors.RecipientPhoneRequired);

        return Result<ShippingRecipient>.Success(new ShippingRecipient(
            firstName.Trim(),
            lastName.Trim(),
            phone.Trim(),
            string.IsNullOrWhiteSpace(email) ? null : email.Trim()));
    }
}