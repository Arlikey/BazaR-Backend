namespace BazaR.Backend.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; init; } = "BazaR";
    public string Audience { get; init; } = "BazaR";
    public string SecretKey { get; init; } = default!;
    public int AccessMinutes { get; init; } = 30;
}
