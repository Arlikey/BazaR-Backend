using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BazaR.Backend.Application.Common.Abstractions.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BazaR.Backend.Infrastructure.Auth;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _opt;

    public JwtProvider(IOptions<JwtOptions> options) => _opt = options.Value;

    public (string token, DateTimeOffset expiresAt) CreateAccessToken(Guid userId, IReadOnlyCollection<string> roles)
    {
        var expires = DateTimeOffset.UtcNow.AddMinutes(_opt.AccessMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires.UtcDateTime,
            signingCredentials: creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, expires);
    }

    public string CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public string HashRefreshToken(string refreshTokenRaw)
    {
        // SHA256 достаточно для хеша refresh (а пароль хешится отдельно hasher'ом)
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(refreshTokenRaw);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}
