using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetById(UserId id)
        => _db.Users.FirstOrDefaultAsync(x => x.Id == id);

    public Task<User?> GetByEmail(Email email)
        => _db.Users.FirstOrDefaultAsync(x => x.Email.Value == email.Value);

    public Task Add(User user)
    {
        _db.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task Update(User user)
    {
        _db.Users.Update(user);
        return Task.CompletedTask;
    }
}
