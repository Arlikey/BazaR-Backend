namespace BazaR.Backend.Domain.Categories;

public readonly record struct CategoryAttributeId(Guid Value)
{
    public static CategoryAttributeId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
