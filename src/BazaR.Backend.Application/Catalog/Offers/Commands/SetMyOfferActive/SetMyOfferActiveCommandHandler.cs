using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Sales.Offers.Commands.SetMyOfferActive;

public sealed class SetMyOfferActiveCommandHandler : IRequestHandler<SetMyOfferActiveCommand, Result>
{
    private readonly IOfferRepository _offers;
    private readonly IProductRepository _products;
    private readonly ISellerRepository _sellers;
    private readonly ICurrentUser _current;
    private readonly IUnitOfWork _uow;

    public SetMyOfferActiveCommandHandler(
        IOfferRepository offers,
        IProductRepository products,
        ISellerRepository sellers,
        ICurrentUser current,
        IUnitOfWork uow)
    {
        _offers = offers;
        _products = products;
        _sellers = sellers;
        _current = current;
        _uow = uow;
    }

    public async Task<Result> Handle(SetMyOfferActiveCommand request, CancellationToken ct)
    {
        // Проверяем, аутентифицирован ли пользователь
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        // Получаем продавца по текущему пользователю
        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller not found."));

        // Продавец должен быть активным
        if (seller.Status != SellerStatus.Active)
            return Result.Failure(new Error("Seller.NotActive", "Seller must be active."));

        var productId = new ProductId(request.ProductId);
        // Загружаем продукт
        var product = await _products.GetByIdAsync(productId, ct);
        if (product is null)
            return Result.Failure(new Error("Product.NotFound", "Product not found."));

        // Проверяем, что продукт принадлежит данному продавцу
        if (product.OwnerSellerId != seller.Id)
            return Result.Failure(new Error("Auth.Forbidden", "Not your product."));

        // Получаем оффер для этого продукта и продавца
        var offer = await _offers.GetByProductAndSellerAsync(productId, seller.Id, ct);
        if (offer is null)
            return Result.Failure(new Error("Offer.NotFound", "Offer not found. Create it first."));

        Result res;

        if (request.IsActive)
        {
            // Активируем или возобновляем оффер в зависимости от текущего статуса
            res = offer.Status == OfferStatus.Paused ? offer.Resume() : offer.Activate();
            if (res.IsFailure) return res;

            // Публикуем продукт, чтобы он стал виден покупателям
            var pubRes = product.Publish();
            if (pubRes.IsFailure) return pubRes;
        }
        else
        {
            // Приостанавливаем оффер
            res = offer.Pause();
            if (res.IsFailure) return res;

            // Убираем продукт из публичного доступа
            var unpubRes = product.Unpublish();
            if (unpubRes.IsFailure) return unpubRes;
        }

        // Сохраняем изменения в обоих агрегатах
        _offers.Update(offer);
        _products.Update(product);

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}