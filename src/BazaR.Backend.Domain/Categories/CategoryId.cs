namespace BazaR.Backend.Domain.Categories;

public readonly record struct CategoryId(Guid Value)
{
    public static CategoryId New() => new(Guid.NewGuid());
}
