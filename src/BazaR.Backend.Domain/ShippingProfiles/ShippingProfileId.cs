namespace BazaR.Backend.Domain.Shipping;

public readonly record struct ShippingProfileId(Guid Value)
{
    public static ShippingProfileId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}