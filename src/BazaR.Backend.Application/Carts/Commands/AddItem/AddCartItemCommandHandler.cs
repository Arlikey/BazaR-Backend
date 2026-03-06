using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.AddItem;

public sealed class AddCartItemCommandHandler
    : IRequestHandler<AddCartItemCommand, Result>
{
    private readonly ICartRepository _carts;
    private readonly IOfferRepository _offers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public AddCartItemCommandHandler(
        ICartRepository carts,
        IOfferRepository offers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _carts = carts;
        _offers = offers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(AddCartItemCommand request, CancellationToken ct)
    {
        // 1. Проверка аутентификации пользователя
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        // 2. Валидация входных данных
        if (request.OfferId == Guid.Empty)
            return Result.Failure(new Error("Offer.InvalidId", "OfferId is invalid."));

        if (request.Quantity <= 0)
            return Result.Failure(new Error("Cart.Quantity.Invalid", "Quantity must be greater than zero."));

        var userId = new UserId(_current.UserId);

        // 3. Загружаем оффер, чтобы проверить его существование и получить цену
        var offer = await _offers.GetByIdAsync(new OfferId(request.OfferId), ct);
        if (offer is null)
            return Result.Failure(new Error("Offer.NotFound", "Offer not found."));

        if (offer.Price is null)
            return Result.Failure(new Error("Offer.Price.Missing", "Offer price is missing."));

        // 4. Пытаемся получить активную корзину текущего пользователя
        var cart = await _carts.GetActiveByUserIdAsync(userId, ct);
        var isNewCart = false;

        // 5. Если корзины нет – создаём новую
        if (cart is null)
        {
            var createRes = Cart.Create(userId);
            if (createRes.IsFailure)
                return createRes;

            cart = createRes.Value!;
            _carts.Add(cart);
            isNewCart = true;
        }

        // 6. Добавляем товар в корзину (доменная логика)
        var addRes = cart.AddItem(
            offer.Id,
            request.Quantity,
            new MoneySnapshot(offer.Price.Amount, offer.Price.Currency));

        if (addRes.IsFailure)
            return addRes;

        // 7. Если корзина уже существовала – помечаем как изменённую 
        if (!isNewCart)
            _carts.Update(cart);

        // 8. Сохраняем изменения в базе данных
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}