
public sealed record TokenPair(string AccessToken, string RefreshToken);

public interface ITokenService
{
    TokenPair IssueTokens(Guid userId, bool isAdmin, IReadOnlyCollection<string> roles);

  
    string HashRefreshToken(string refreshToken);
    bool VerifyRefreshToken(string refreshToken, string refreshTokenHash);
    DateTimeOffset GetRefreshTokenExpiryUtc();
}