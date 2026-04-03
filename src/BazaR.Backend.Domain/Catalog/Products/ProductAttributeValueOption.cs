namespace BazaR.Backend.Domain.Catalog.Products;

public sealed class ProductAttributeValueOption
{
    public Guid OptionId { get; private set; }

    private ProductAttributeValueOption() { }

    internal ProductAttributeValueOption(Guid optionId)
    {
        OptionId = optionId;
    }
}