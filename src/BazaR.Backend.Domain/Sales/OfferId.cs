namespace BazaR.Backend.Domain.Sales;

public readonly record struct OfferId(Guid Value)
{
    public static OfferId New() => new(Guid.NewGuid());
}
