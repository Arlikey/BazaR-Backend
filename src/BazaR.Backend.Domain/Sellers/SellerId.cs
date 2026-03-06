namespace BazaR.Backend.Domain.Sellers;

public readonly record struct SellerId(Guid Value)
{
    public static SellerId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public enum SellerType
{
    Regular = 0,
    Platform = 1 // сама Bаza-R
}

public enum SellerStatus
{
    Draft = 0,
    PendingApproval = 1,
    Active = 2,
    Suspended = 3,
    Closed = 4
}
