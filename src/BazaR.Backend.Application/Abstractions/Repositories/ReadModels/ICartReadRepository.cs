namespace BazaR.Backend.Application.Abstractions.Repositories.ReadModels;

using BazaR.Backend.Application.Carts.DTOs;
public interface ICartReadRepository
{
    Task<CustomerCartDto?> GetCustomerActiveCartAsync(Guid userId, CancellationToken ct = default);
    Task<CartReadModel?> GetByIdAsync(Guid cartId, CancellationToken ct = default);

    // админка: список корзин
    Task<CartPagedResult<CartAdminListItemReadModel>> SearchAsync(
    CartFilter filter,
    Pagination pagination,
    CancellationToken ct = default);
}