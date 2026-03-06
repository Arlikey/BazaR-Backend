namespace BazaR.Backend.Application.Identity;

public sealed record AuthResponse(
    Guid UserId,
    string AccessToken,
    DateTimeOffset AccessExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshExpiresAt);
