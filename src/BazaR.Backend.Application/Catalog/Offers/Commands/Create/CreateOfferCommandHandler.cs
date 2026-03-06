using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Create;

public sealed class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Result<Guid>>
{
    private readonly IOfferRepository _offers;
    private readonly ISellerRepository _sellers;
    private readonly IProductRepository _products;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public CreateOfferCommandHandler(
        IOfferRepository offers,
        ISellerRepository sellers,
        IProductRepository products,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _offers = offers;
        _sellers = sellers;
        _products = products;
        _uow = uow;
        _current = current;
    }

    public async Task<Result<Guid>> Handle(CreateOfferCommand request, CancellationToken ct)
    {
        // Аутентификация
        if (!_current.IsAuthenticated)
            return Result<Guid>.Failure(new Error("Auth.Required", "Authentication required."));

        // Валидация входных данных
        if (request.ProductId == Guid.Empty)
            return Result<Guid>.Failure(new Error("Offer.ProductRequired", "ProductId is required."));
        if (request.Stock < 0)
            return Result<Guid>.Failure(new Error("Offer.StockCannotBeNegative", "Stock cannot be negative."));
        if (request.MinOrderQuantity < 1)
            return Result<Guid>.Failure(new Error("Offer.MinOrderQuantityInvalid", "MinOrderQuantity must be at least 1."));

        // Получаем продавца текущего пользователя
        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result<Guid>.Failure(new Error("Seller.NotFound", "Seller not found."));
        if (seller.Status != SellerStatus.Active)
            return Result<Guid>.Failure(new Error("Seller.NotActive", "Seller must be active."));

        var productId = new ProductId(request.ProductId);

        // Проверяем, что продукт существует и принадлежит продавцу
        var product = await _products.GetByIdForOwnerAsync(productId, seller.Id, ct);
        if (product is null)
            return Result<Guid>.Failure(new Error("Product.NotFound", "Product not found for this seller."));

        // Проверяем уникальность оффера для данного продукта и продавца
        var exists = await _offers.GetByProductAndSellerAsync(productId, seller.Id, ct);
        if (exists is not null)
            return Result<Guid>.Failure(new Error("Offer.AlreadyExists", "Offer already exists for this product and seller."));

        // Создаём оффер в статусе Draft
        var offerRes = Offer.Create(productId, seller.Id, request.Stock);
        if (offerRes.IsFailure)
            return Result<Guid>.Failure(offerRes.Error);
        var offer = offerRes.Value!;

        // Устанавливаем цену
        var priceRes = offer.SetPrice(request.PriceAmount, request.PriceCurrency);
        if (priceRes.IsFailure)
            return Result<Guid>.Failure(priceRes.Error);

        // Устанавливаем старую цену (если указана)
        if (request.OldPriceAmount is not null)
        {
            var cur = string.IsNullOrWhiteSpace(request.OldPriceCurrency)
                ? request.PriceCurrency
                : request.OldPriceCurrency!;
            var oldRes = offer.SetOldPrice(request.OldPriceAmount.Value, cur);
            if (oldRes.IsFailure)
                return Result<Guid>.Failure(oldRes.Error);
        }

        // Устанавливаем артикул продавца, срок доставки и минимальное количество заказа
        var skuRes = offer.SetSellerSku(request.SellerSku);
        if (skuRes.IsFailure) return Result<Guid>.Failure(skuRes.Error);
        var ddRes = offer.SetDeliveryDays(request.DeliveryDays);
        if (ddRes.IsFailure) return Result<Guid>.Failure(ddRes.Error);
        var moqRes = offer.SetMinOrderQuantity(request.MinOrderQuantity);
        if (moqRes.IsFailure) return Result<Guid>.Failure(moqRes.Error);

        // Если требуется активация оффера и публикация продукта
        if (request.Activate)
        {
            var actRes = offer.Activate();
            if (actRes.IsFailure)
                return Result<Guid>.Failure(actRes.Error);

            var pubRes = product.Publish();
            if (pubRes.IsFailure)
                return Result<Guid>.Failure(pubRes.Error);

            _products.Update(product);
        }

        _offers.Add(offer);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(offer.Id.Value);
    }
}