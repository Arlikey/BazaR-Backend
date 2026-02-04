using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales.Events;

namespace BazaR.Backend.Domain.Sales;

public sealed class Offer : AggregateRoot<OfferId>
{
    public ProductId ProductId { get; private set; }
    public SellerId SellerId { get; private set; }

    public Money? Price { get; private set; }
    public int Stock { get; private set; }

    public OfferStatus Status { get; private set; }

    private Offer(OfferId id, ProductId productId, SellerId sellerId, int initialStock) : base(id)
    {
        ProductId = productId;
        SellerId = sellerId;

        Stock = initialStock;
        Status = OfferStatus.Draft;
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
        AddDomainEvent(new OfferPriceChangedEvent(Id, Price.Amount, Price.Currency));
        return Result.Success();
    }

    public Result IncreaseStock(int amount)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (amount <= 0)
            return Result.Failure(OfferErrors.AmountMustBePositive);

        Stock += amount;
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
        AddDomainEvent(new OfferStockDecreasedEvent(Id, amount, Stock));

        // автопауза если закончился товар и оффер был активен
        if (Stock == 0 && Status == OfferStatus.Active)
        {
            Status = OfferStatus.Paused;
            AddDomainEvent(new OfferPausedEvent(Id));
        }

        return Result.Success();
    }

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
        AddDomainEvent(new OfferActivatedEvent(Id));
        return Result.Success();
    }

    /// <summary>
    /// Временно выключить продажу.
    /// </summary>
    public Result Pause()
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (Status != OfferStatus.Active)
            return Result.Success();

        Status = OfferStatus.Paused;
        AddDomainEvent(new OfferPausedEvent(Id));
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == OfferStatus.Archived)
            return Result.Success();

        Status = OfferStatus.Archived;
        AddDomainEvent(new OfferArchivedEvent(Id));
        return Result.Success();
    }
}
