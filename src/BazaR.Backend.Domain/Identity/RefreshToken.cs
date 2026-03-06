namespace BazaR.Backend.Domain.Identity;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public bool IsActive => RevokedAt is null && DateTimeOffset.UtcNow < ExpiresAt;

    private RefreshToken() { }

    private RefreshToken(Guid id, string tokenHash, DateTimeOffset createdAt, DateTimeOffset expiresAt)
    {
        Id = id;
        TokenHash = tokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(string tokenHash, TimeSpan ttl)
        => new(Guid.NewGuid(), tokenHash, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.Add(ttl));

    public void Revoke() => RevokedAt ??= DateTimeOffset.UtcNow;
}
