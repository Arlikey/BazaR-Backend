using BazaR.Backend.Domain.Identity;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetByIdAsync(AuthUserId id, CancellationToken ct = default);
    Task<AuthUser?> GetByEmailAsync(string email, CancellationToken ct = default);  
    Task<AuthUser?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    void Add(AuthUser user);
    void Update(AuthUser user);
}