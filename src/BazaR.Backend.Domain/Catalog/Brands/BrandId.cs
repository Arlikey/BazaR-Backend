namespace BazaR.Backend.Domain.Brands;

public readonly record struct BrandId(Guid Value)
{
    public static BrandId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}