using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserId(UserId userId);

    Task Add(Cart cart);
    Task Update(Cart cart);
}
