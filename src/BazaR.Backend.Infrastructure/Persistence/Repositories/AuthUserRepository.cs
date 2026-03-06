using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class AuthUserRepository : IAuthUserRepository
{
    private readonly AppDbContext _db;
    public AuthUserRepository(AppDbContext db) => _db = db;

    public async Task<AuthUser?> GetByIdAsync(AuthUserId id, CancellationToken ct = default)
        => await _db.AuthUsers
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);

    public async Task<AuthUser?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var normalized = email.Trim().ToLowerInvariant();

        return await _db.AuthUsers
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Email.Value == normalized, ct);
    }

    public Task<AuthUser?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => _db.AuthUsers
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public void Add(AuthUser user) => _db.AuthUsers.Add(user);
    public void Update(AuthUser user) => _db.AuthUsers.Update(user);
}
