namespace BazaR.Backend.Domain.Shipping;

public readonly record struct ShippingMethodConfigId(Guid Value)
{
    public static ShippingMethodConfigId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}