using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface ICheckoutRepository
{
    Task<Checkout?> GetByIdAsync(CheckoutId id, CancellationToken ct);
    Task<Checkout?> GetDraftByCartIdAsync(CartId cartId, CancellationToken ct);
    Task<Checkout?> GetActiveDraftByUserIdAsync(UserId userId, CancellationToken ct);

    void Add(Checkout checkout);
    void Update(Checkout checkout);
}