using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Products;

namespace BazaR.Backend.Domain.ViewedProducts;
public sealed class ViewedProduct
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public ProductId ProductId { get; private set; } = default!;
    public DateTime ViewedAtUtc { get; private set; }

    private ViewedProduct()
    {
    }

    private ViewedProduct(Guid id, Guid userId, ProductId productId, DateTime viewedAtUtc)
    {
        Id = id;
        UserId = userId;
        ProductId = productId;
        ViewedAtUtc = viewedAtUtc;
    }

    public static ViewedProduct Create(Guid userId, ProductId productId)
    {
        return new ViewedProduct(
            Guid.NewGuid(),
            userId,
            productId,
            DateTime.UtcNow);
    }

    public void MarkViewed()
    {
        ViewedAtUtc = DateTime.UtcNow;
    }
}