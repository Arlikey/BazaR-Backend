namespace BazaR.Backend.Domain.Identity;

public readonly record struct AuthUserId(Guid Value)
{
    public static AuthUserId New() => new(Guid.NewGuid());
}
