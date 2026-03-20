namespace BazaR.Backend.Domain.PaymentProfiles;

public readonly record struct PaymentMethodConfigId(Guid Value)
{
    public static PaymentMethodConfigId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}