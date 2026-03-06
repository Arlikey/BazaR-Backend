using System.Text.RegularExpressions;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Identity;

public sealed class IdentityEmail
{
    private static readonly Regex Rx = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; private set; } = default!;

    
    private IdentityEmail() { }

    private IdentityEmail(string value)
    {
        Value = value.Trim().ToLowerInvariant(); // сразу нормализуем
    }

    public static Result<IdentityEmail> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<IdentityEmail>.Failure(new Error("Identity.EmailRequired", "Email is required."));

        var trimmed = email.Trim();
        if (!Rx.IsMatch(trimmed))
            return Result<IdentityEmail>.Failure(new Error("Identity.EmailInvalid", "Email is invalid."));

        return Result<IdentityEmail>.Success(new IdentityEmail(trimmed));
    }

    public override string ToString() => Value;

   
    public static implicit operator string(IdentityEmail email) => email.Value;
}