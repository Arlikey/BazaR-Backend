namespace BazaR.Backend.Domain.Shippings;

public readonly record struct ShippingId(Guid Value)
{
    public static ShippingId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}