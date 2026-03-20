namespace BazaR.Backend.Domain.Checkouts;

public readonly record struct CheckoutLineId(Guid Value)
{
    public static CheckoutLineId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}