using BazaR.Backend.Application.Abstractions.Repositories.ReadModels;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Carts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Queries.GetMyActiveCart;

public sealed class GetMyActiveCartQueryHandler
    : IRequestHandler<GetMyActiveCartQuery, Result<CustomerCartDto?>>
{
    private readonly ICartReadRepository _carts;
    private readonly ICurrentUser _current;

    public GetMyActiveCartQueryHandler(
        ICartReadRepository carts,
        ICurrentUser current)
    {
        _carts = carts;
        _current = current;
    }

    public async Task<Result<CustomerCartDto?>> Handle(
        GetMyActiveCartQuery request,
        CancellationToken ct)
    {
        // Проверяем, аутентифицирован ли пользователь
        if (!_current.IsAuthenticated)
            return Result<CustomerCartDto?>
                .Failure(new Error("Auth.Required", "Authentication required."));

        // Получаем детальную информацию об активной корзине текущего пользователя
        var cart = await _carts.GetCustomerActiveCartAsync(_current.UserId, ct);

        // Возвращаем корзину (может быть null, если корзины нет)
        return Result<CustomerCartDto?>.Success(cart);
    }
}