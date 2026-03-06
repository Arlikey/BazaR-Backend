using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales.Events;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Domain.Sales;

public sealed class Offer : AggregateRoot<OfferId>
{
    public ProductId ProductId { get; private set; }
    public SellerId SellerId { get; private set; }

    public Money? Price { get; private set; }
    public Money? OldPrice { get; private set; }          //  старая цена (для скидок)
    public int Stock { get; private set; }

    public string? SellerSku { get; private set; }        //  внутренний артикул продавца
    public int? DeliveryDays { get; private set; }        //  срок доставки в днях
    public int MinOrderQuantity { get; private set; } = 1; //  минимальное количество для заказа

    public OfferStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Offer(OfferId id, ProductId productId, SellerId sellerId, int initialStock) : base(id)
    {
        ProductId = productId;
        SellerId = sellerId;
        Stock = initialStock;
        Status = OfferStatus.Draft;
        MinOrderQuantity = 1; 
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    private Offer() { } 

    public static Result<Offer> Create(ProductId productId, SellerId sellerId, int initialStock)
    {
        if (productId.Value == default)
            return Result<Offer>.Failure(OfferErrors.ProductRequired);

        if (sellerId.Value == default)
            return Result<Offer>.Failure(OfferErrors.SellerRequired);

        if (initialStock < 0)
            return Result<Offer>.Failure(OfferErrors.StockCannotBeNegative);

        var offer = new Offer(OfferId.New(), productId, sellerId, initialStock);
        offer.AddDomainEvent(new OfferCreatedEvent(offer.Id, offer.ProductId, offer.SellerId));
        return Result<Offer>.Success(offer);
    }

    private Result EnsureNotArchived()
        => Status == OfferStatus.Archived
            ? Result.Failure(OfferErrors.ArchivedCannotBeModified)
            : Result.Success();

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    // ========== Цены ==========
    public Result SetPrice(decimal amount, string currency = "UAH")
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        var moneyRes = Money.Create(amount, currency);
        if (moneyRes.IsFailure)
            return Result.Failure(moneyRes.Error);

        var newPrice = moneyRes.Value!;
        if (Price is not null && Price.Equals(newPrice))
            return Result.Success();

        Price = newPrice;
        Touch();
        AddDomainEvent(new OfferPriceChangedEvent(Id, Price.Amount, Price.Currency));
        return Result.Success();
    }

    public Result SetOldPrice(decimal amount, string currency = "UAH")
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        var moneyRes = Money.Create(amount, currency);
        if (moneyRes.IsFailure)
            return Result.Failure(moneyRes.Error);

        OldPrice = moneyRes.Value;
        Touch();
      
        return Result.Success();
    }

    public Result ClearOldPrice()
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (OldPrice is null)
            return Result.Success();

        OldPrice = null;
        Touch();
        return Result.Success();
    }

    // ========== Остаток ==========
    public Result SetStock(int newStock)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (newStock < 0)
            return Result.Failure(OfferErrors.StockCannotBeNegative);

        if (Stock == newStock)
            return Result.Success();

        var old = Stock;
        Stock = newStock;
        Touch();

        if (newStock > old)
            AddDomainEvent(new OfferStockIncreasedEvent(Id, newStock - old, Stock));
        else
            AddDomainEvent(new OfferStockDecreasedEvent(Id, old - newStock, Stock));

        // автопауза если закончился товар и оффер был активен
        if (Stock == 0 && Status == OfferStatus.Active)
        {
            Status = OfferStatus.Paused;
            AddDomainEvent(new OfferPausedEvent(Id));
        }

        return Result.Success();
    }

    public Result IncreaseStock(int amount)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (amount <= 0)
            return Result.Failure(OfferErrors.AmountMustBePositive);

        Stock += amount;
        Touch();
        AddDomainEvent(new OfferStockIncreasedEvent(Id, amount, Stock));
        return Result.Success();
    }

    public Result DecreaseStock(int amount)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (amount <= 0)
            return Result.Failure(OfferErrors.AmountMustBePositive);

        var newStock = Stock - amount;
        if (newStock < 0)
            return Result.Failure(OfferErrors.StockCannotBeNegative);

        Stock = newStock;
        Touch();
        AddDomainEvent(new OfferStockDecreasedEvent(Id, amount, Stock));

        if (Stock == 0 && Status == OfferStatus.Active)
        {
            Status = OfferStatus.Paused;
            AddDomainEvent(new OfferPausedEvent(Id));
        }

        return Result.Success();
    }

    // ========== Артикул продавца ==========
    public Result SetSellerSku(string? sku)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (SellerSku == sku)
            return Result.Success();

        SellerSku = sku?.Trim(); 
        Touch();
        return Result.Success();
    }

    // ========== Срок доставки ==========
    public Result SetDeliveryDays(int? days)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (days.HasValue && days.Value < 0)
            return Result.Failure(OfferErrors.DeliveryDaysCannotBeNegative);

        if (DeliveryDays == days)
            return Result.Success();

        DeliveryDays = days;
        Touch();
        return Result.Success();
    }

    // ========== Минимальное количество ==========
    public Result SetMinOrderQuantity(int quantity)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (quantity < 1)
            return Result.Failure(OfferErrors.MinOrderQuantityMustBeAtLeastOne);

        if (MinOrderQuantity == quantity)
            return Result.Success();

        MinOrderQuantity = quantity;
        Touch();
        return Result.Success();
    }

    // ========== Управление статусом ==========
    public Result Activate()
    {
        if (Status == OfferStatus.Archived)
            return Result.Failure(OfferErrors.CannotActivateArchived);

        if (Status == OfferStatus.Active)
            return Result.Success();

        if (Price is null)
            return Result.Failure(OfferErrors.CannotActivateWithoutPrice);

        if (Stock <= 0)
            return Result.Failure(OfferErrors.CannotActivateWithoutStock);

        Status = OfferStatus.Active;
        Touch();
        AddDomainEvent(new OfferActivatedEvent(Id));
        return Result.Success();
    }

    public Result Pause()
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (Status != OfferStatus.Active)
            return Result.Success();

        Status = OfferStatus.Paused;
        Touch();
        AddDomainEvent(new OfferPausedEvent(Id));
        return Result.Success();
    }

    public Result Resume()
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (Status != OfferStatus.Paused)
            return Result.Success();

        if (Price is null)
            return Result.Failure(OfferErrors.CannotActivateWithoutPrice);

        if (Stock <= 0)
            return Result.Failure(OfferErrors.CannotActivateWithoutStock);

        Status = OfferStatus.Active;
        Touch();
        AddDomainEvent(new OfferActivatedEvent(Id));
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == OfferStatus.Archived)
            return Result.Success();

        Status = OfferStatus.Archived;
        Touch();
        AddDomainEvent(new OfferArchivedEvent(Id));
        return Result.Success();
    }
}