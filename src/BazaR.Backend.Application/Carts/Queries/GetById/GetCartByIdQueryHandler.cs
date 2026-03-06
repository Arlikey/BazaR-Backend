using BazaR.Backend.Application.Abstractions.Repositories.ReadModels;
using BazaR.Backend.Application.Carts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Queries.GetById;

public sealed class GetCartByIdQueryHandler
    : IRequestHandler<GetCartByIdQuery, Result<CartReadModel?>>
{
    private readonly ICartReadRepository _carts;

    public GetCartByIdQueryHandler(ICartReadRepository carts)
    {
        _carts = carts;
    }

    public async Task<Result<CartReadModel?>> Handle(
        GetCartByIdQuery request,
        CancellationToken ct)
    {
        // Проверяем корректность переданного идентификатора корзины
        if (request.CartId == Guid.Empty)
            return Result<CartReadModel?>
                .Failure(new Error("Cart.InvalidId", "CartId is invalid."));

        // Получаем корзину из read-репозитория
        var cart = await _carts.GetByIdAsync(request.CartId, ct);

        // Возвращаем результат (может быть null, если корзина не найдена)
        return Result<CartReadModel?>.Success(cart);
    }
}