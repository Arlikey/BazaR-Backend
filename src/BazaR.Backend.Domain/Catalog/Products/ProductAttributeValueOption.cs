using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Products;

public sealed class ProductAttributeValueOption : Entity<Guid>
{
    public Guid OptionId { get; private set; }

    private ProductAttributeValueOption() { }

    private ProductAttributeValueOption(Guid id, Guid optionId) : base(id)
    {
        OptionId = optionId;
    }

    public static ProductAttributeValueOption Create(Guid optionId)
        => new(Guid.NewGuid(), optionId);
}