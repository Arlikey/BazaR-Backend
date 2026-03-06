using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Users;

public sealed class FullName : ValueObject
{
    public const int MaxPartLength = 100;

    public string FirstName { get; }
    public string LastName { get; }

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<FullName> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<FullName>.Failure(UserErrors.FirstNameRequired);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<FullName>.Failure(UserErrors.LastNameRequired);

        var fn = firstName.Trim();
        var ln = lastName.Trim();

        if (fn.Length > MaxPartLength) return Result<FullName>.Failure(UserErrors.FirstNameTooLong);
        if (ln.Length > MaxPartLength) return Result<FullName>.Failure(UserErrors.LastNameTooLong);

        return Result<FullName>.Success(new FullName(fn, ln));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString() => $"{FirstName} {LastName}";
}
