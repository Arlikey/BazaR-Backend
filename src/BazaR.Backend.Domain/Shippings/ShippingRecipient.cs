using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Shippings;

public sealed record ShippingRecipient
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string? Email { get; private set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    private ShippingRecipient() { }

    private ShippingRecipient(
        string firstName,
        string lastName,
        string phone,
        string? email)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
    }

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
            Normalize(email)));
    }

    public Result Update(
        string firstName,
        string lastName,
        string phone,
        string? email)
    {
        var createResult = Create(firstName, lastName, phone, email);
        if (createResult.IsFailure)
            return Result.Failure(createResult.Error);

        var value = createResult.Value!;
        FirstName = value.FirstName;
        LastName = value.LastName;
        Phone = value.Phone;
        Email = value.Email;

        return Result.Success();
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}