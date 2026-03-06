using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using System.Text.RegularExpressions;

public sealed class Email : ValueObject
{
    public const int MaxLength = 320;

    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; private set; } = default!;

    private Email() { } 

    private Email(string normalized)
    {
        Value = normalized;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Email>.Failure(UserErrors.EmailInvalid);

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
            return Result<Email>.Failure(UserErrors.EmailInvalid);

        if (!EmailRegex.IsMatch(normalized))
            return Result<Email>.Failure(UserErrors.EmailInvalid);

        return Result<Email>.Success(new Email(normalized));
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
