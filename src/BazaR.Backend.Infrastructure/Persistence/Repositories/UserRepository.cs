using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default)
        => await _db.Users
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = NormalizeEmail(email);
        if (normalized is null) return null;

        return await _db.Users
            .FirstOrDefaultAsync(x => x.Email.Value == normalized, ct);
    }

    public async Task<User?> GetByPhoneAsync(string phone, CancellationToken ct = default)
    {
        var normalized = NormalizePhone(phone);
        if (normalized is null) return null;

        return await _db.Users
            .FirstOrDefaultAsync(x => x.Phone != null && x.Phone.Value == normalized, ct);
    }

    public async Task<bool> EmailExistsAsync(string email, UserId? excludeUserId = null, CancellationToken ct = default)
    {
        var normalized = NormalizeEmail(email);
        if (normalized is null) return false;

        var q = _db.Users.AsQueryable();

        if (excludeUserId is not null)
            q = q.Where(x => x.Id != excludeUserId);

        return await q.AnyAsync(x => x.Email.Value == normalized, ct);
    }

    public async Task<bool> PhoneExistsAsync(string phone, UserId? excludeUserId = null, CancellationToken ct = default)
    {
        var normalized = NormalizePhone(phone);
        if (normalized is null) return false;

        var q = _db.Users.AsQueryable();

        if (excludeUserId is not null)
            q = q.Where(x => x.Id != excludeUserId);

        return await q.AnyAsync(x => x.Phone != null && x.Phone.Value == normalized, ct);
    }

    public void Add(User user) => _db.Users.Add(user);

    public void Update(User user) => _db.Users.Update(user);

    public void Remove(User user) => _db.Users.Remove(user);

    private static string? NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return email.Trim().ToLowerInvariant();
    }

    private static string? NormalizePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return null;
        return phone.Trim();
    }
}
