namespace BazaR.Backend.Domain.PaymentProfiles;

public readonly record struct PaymentProfileId(Guid Value)
{
    public static PaymentProfileId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}