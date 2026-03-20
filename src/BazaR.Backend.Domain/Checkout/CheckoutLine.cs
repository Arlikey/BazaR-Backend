using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Domain.Checkouts;

public sealed class CheckoutLine
{
    private CheckoutLine() { }

    private CheckoutLine(
        CheckoutLineId id,
        OfferId offerId,
        ProductId productId,
        SellerId sellerId,
        string productTitle,
        string sku,
        int quantity,
        Money unitPrice)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        Id = id;
        OfferId = offerId;
        ProductId = productId;
        SellerId = sellerId;
        ProductTitle = string.IsNullOrWhiteSpace(productTitle)
            ? throw new InvalidOperationException("Product title is required.")
            : productTitle.Trim();
        Sku = (sku ?? string.Empty).Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public CheckoutLineId Id { get; private set; }
    public OfferId OfferId { get; private set; }
    public ProductId ProductId { get; private set; }
    public SellerId SellerId { get; private set; }

    public string ProductTitle { get; private set; } = default!;
    public string Sku { get; private set; } = default!;

    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = default!;

    public Money LineTotal => UnitPrice * Quantity;

    public RecipientInfo? Recipient { get; private set; }
    public ShippingSelection? Shipping { get; private set; }
    public PaymentSelection? Payment { get; private set; }

    public static CheckoutLine Create(
        CheckoutLineId id,
        OfferId offerId,
        ProductId productId,
        SellerId sellerId,
        string productTitle,
        string sku,
        int quantity,
        Money unitPrice)
    {
        return new CheckoutLine(
            id,
            offerId,
            productId,
            sellerId,
            productTitle,
            sku,
            quantity,
            unitPrice);
    }

    public void SetRecipient(RecipientInfo recipient)
    {
        Recipient = recipient ?? throw new InvalidOperationException("Recipient is required.");
    }

    public void SetShipping(ShippingSelection shipping)
    {
        Shipping = shipping ?? throw new InvalidOperationException("Shipping is required.");
    }

    public void SetPayment(PaymentSelection payment)
    {
        Payment = payment ?? throw new InvalidOperationException("Payment is required.");
    }

    public bool IsComplete()
        => Recipient is not null
           && Shipping is not null
           && Payment is not null;
}