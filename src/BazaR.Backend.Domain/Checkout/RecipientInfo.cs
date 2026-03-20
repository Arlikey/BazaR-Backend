namespace BazaR.Backend.Domain.Checkouts;

public sealed record RecipientInfo(
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    bool IsCustomerRecipient)
{
    public string FullName => $"{FirstName} {LastName}".Trim();

    public static RecipientInfo Create(
        string firstName,
        string lastName,
        string phone,
        string? email,
        bool isCustomerRecipient)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new InvalidOperationException("Recipient first name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new InvalidOperationException("Recipient last name is required.");

        if (string.IsNullOrWhiteSpace(phone))
            throw new InvalidOperationException("Recipient phone is required.");

        return new RecipientInfo(
            firstName.Trim(),
            lastName.Trim(),
            phone.Trim(),
            string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            isCustomerRecipient);
    }
}