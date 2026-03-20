namespace BazaR.Backend.Domain.Checkouts;

public readonly record struct CheckoutId(Guid Value)
{
    public static CheckoutId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}