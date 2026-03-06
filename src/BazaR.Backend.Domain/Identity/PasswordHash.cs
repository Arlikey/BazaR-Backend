using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Identity;

public sealed class PasswordHash : ValueObject
{
    private PasswordHash(string value) => Value = value;

    public string Value { get; }

    public static Result<PasswordHash> Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return Result<PasswordHash>.Failure(new Error("Identity.PasswordHashRequired", "Password hash is required."));

        return Result<PasswordHash>.Success(new PasswordHash(hash.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
