using System.Net.Mail;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sellers;

public sealed record EmailAddress
{
    public string Value { get; }
    private EmailAddress(string value) => Value = value;

    public static Result<EmailAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<EmailAddress>.Failure(new Error("Seller.EmailRequired", "Email is required."));

        var v = value.Trim();
        try { _ = new MailAddress(v); }
        catch { return Result<EmailAddress>.Failure(new Error("Seller.EmailInvalid", "Email format is invalid.")); }

        return Result<EmailAddress>.Success(new EmailAddress(v));
    }
}

