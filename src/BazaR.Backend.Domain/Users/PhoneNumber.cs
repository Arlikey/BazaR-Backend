using BazaR.Backend.Domain.Common;
using System.Text.RegularExpressions;

namespace BazaR.Backend.Domain.Users;

public sealed class PhoneNumber : ValueObject
{
    public const int MaxLength = 30;
    private static readonly Regex Rx = new(@"^\+?[0-9]{7,20}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<PhoneNumber>.Failure(UserErrors.PhoneInvalid);

        var trimmed = value.Trim().Replace(" ", "").Replace("-", "");

        if (trimmed.Length > MaxLength)
            return Result<PhoneNumber>.Failure(UserErrors.PhoneTooLong);

        if (!Rx.IsMatch(trimmed))
            return Result<PhoneNumber>.Failure(UserErrors.PhoneInvalid);

        return Result<PhoneNumber>.Success(new PhoneNumber(trimmed));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
