using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<User?> GetByPhoneAsync(string phone, CancellationToken ct = default);

    Task<bool> EmailExistsAsync(string email, UserId? excludeUserId = null, CancellationToken ct = default);

    Task<bool> PhoneExistsAsync(string phone, UserId? excludeUserId = null, CancellationToken ct = default);

    void Add(User user);

    void Update(User user);

    void Remove(User user);
}
