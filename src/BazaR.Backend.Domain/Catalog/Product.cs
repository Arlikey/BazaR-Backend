using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Catalog.Events;

namespace BazaR.Backend.Domain.Catalog;

public sealed class Product : AggregateRoot<ProductId>
{
    private const int MaxNameLength = 200;
    private const int MaxDescriptionLength = 5000;

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public CategoryId CategoryId { get; private set; }
    public Money? Price { get; private set; }

    public int Stock { get; private set; }
    public bool IsActive { get; private set; }

    private Product(ProductId id, string name, CategoryId categoryId, int stock) : base(id)
    {
        Name = name;
        CategoryId = categoryId;
        Stock = stock;
        IsActive = false;
    }

    private Product() { } 

    
    public static Result<Product> Create(string name, CategoryId categoryId, int initialStock)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Failure(ProductErrors.NameRequired);

        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength)
            return Result<Product>.Failure(ProductErrors.NameTooLong);

        if (categoryId.Value == default)
            return Result<Product>.Failure(ProductErrors.CategoryRequired);

        if (initialStock < 0)
            return Result<Product>.Failure(ProductErrors.StockCannotBeNegative);

        var product = new Product(ProductId.New(), trimmedName, categoryId, initialStock);
        product.AddDomainEvent(new ProductCreatedEvent(product.Id));

        return Result<Product>.Success(product);
    }

    

    public Result ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(ProductErrors.NameRequired);

        var trimmed = name.Trim();
        if (trimmed.Length > MaxNameLength)
            return Result.Failure(ProductErrors.NameTooLong);

        Name = trimmed;
        return Result.Success();
    }

    public Result ChangeDescription(string? description)
    {
        if (description is null)
        {
            Description = null;
            return Result.Success();
        }

        var trimmed = description.Trim();
        if (trimmed.Length > MaxDescriptionLength)
            return Result.Failure(ProductErrors.DescriptionTooLong);

        Description = trimmed.Length == 0 ? null : trimmed;
        return Result.Success();
    }

    public Result ChangeCategory(CategoryId categoryId)
    {
        if (categoryId.Value == default)
            return Result.Failure(ProductErrors.CategoryRequired);

        CategoryId = categoryId;
        return Result.Success();
    }

    public Result SetPrice(decimal amount, string currency = "UAH")
    {
        var moneyRes = Money.Create(amount, currency);
        if (moneyRes.IsFailure)
            return Result.Failure(moneyRes.Error);

        Price = moneyRes.Value!;
        AddDomainEvent(new ProductPriceChangedEvent(Id, Price.Amount, Price.Currency));
        return Result.Success();
    }

    public Result IncreaseStock(int amount)
    {
        if (amount <= 0)
            return Result.Failure(ProductErrors.StockCannotBeNegative); 

        Stock += amount;
        return Result.Success();
    }

    public Result DecreaseStock(int amount)
    {
        if (amount <= 0)
            return Result.Failure(ProductErrors.StockCannotBeNegative);

        var newStock = Stock - amount;
        if (newStock < 0)
            return Result.Failure(ProductErrors.StockCannotBeNegative);

        Stock = newStock;
        return Result.Success();
    }

  
    public Result Activate()
    {
        if (CategoryId.Value == default)
            return Result.Failure(ProductErrors.CannotActivateWithoutCategory);

        if (Price is null)
            return Result.Failure(ProductErrors.CannotActivateWithoutPrice);

        IsActive = true;
        AddDomainEvent(new ProductActivatedEvent(Id));
        return Result.Success();
    }

    public Result Deactivate()
    {
        IsActive = false;
        return Result.Success();
    }
}
