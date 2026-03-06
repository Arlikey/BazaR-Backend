namespace BazaR.Backend.Application.Common.Abstractions.Security;

public sealed record TokenPair(string AccessToken, DateTimeOffset AccessExpiresAt, string RefreshToken, DateTimeOffset RefreshExpiresAt);

public interface IJwtProvider
{
    (string token, DateTimeOffset expiresAt) CreateAccessToken(Guid userId, IReadOnlyCollection<string> roles);
    string CreateRefreshToken(); 
    string HashRefreshToken(string refreshTokenRaw); 
}
