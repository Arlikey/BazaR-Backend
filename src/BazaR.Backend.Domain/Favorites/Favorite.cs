using BazaR.Backend.Domain.Catalog.Products;

using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Favorites;

public sealed class Favorite
{
    public UserId UserId { get; private set; }
    public ProductId ProductId { get; private set; }
    public DateTime AddedAtUtc { get; private set; }

    private Favorite() { }

    private Favorite(UserId userId, ProductId productId, DateTime addedAtUtc)
    {
        UserId = userId;
        ProductId = productId;
        AddedAtUtc = addedAtUtc;
    }

    public static Favorite Create(UserId userId, ProductId productId, DateTime addedAtUtc)
        => new(userId, productId, addedAtUtc);
}