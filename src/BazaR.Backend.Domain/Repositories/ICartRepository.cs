// Application/Abstractions/Repositories/ICartRepository.cs
using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByIdAsync(CartId id, CancellationToken ct = default);

    
    Task<Cart?> GetByUserIdAsync(UserId userId, CancellationToken ct = default);

   
    Task<Cart?> GetActiveByUserIdAsync(UserId userId, CancellationToken ct = default);

    void Add(Cart cart);
    void Update(Cart cart);
}