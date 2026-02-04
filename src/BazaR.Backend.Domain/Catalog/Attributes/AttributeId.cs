namespace BazaR.Backend.Domain.Catalog.Attributes;

public readonly record struct AttributeId(Guid Value)
{
    public static AttributeId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
