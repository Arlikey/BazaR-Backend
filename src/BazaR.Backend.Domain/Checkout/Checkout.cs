using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Checkouts;

public sealed class Checkout : AggregateRoot<CheckoutId>
{
    private readonly List<CheckoutLine> _lines = new();

    private Checkout() { }

    private Checkout(
        CheckoutId id,
        CartId cartId,
        UserId userId,
        IEnumerable<CheckoutLine> lines,
        DateTime createdAtUtc) : base(id)
    {
        var list = lines?.ToList() ?? new List<CheckoutLine>();
        if (list.Count == 0)
            throw new InvalidOperationException("Checkout cannot be started without lines.");

        CartId = cartId;
        UserId = userId;
        Status = CheckoutStatus.Draft;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;

        _lines.AddRange(list);

        RecalculateTotals();
    }

    public CartId CartId { get; private set; } = default!;
    public UserId UserId { get; private set; } = default!;
    public CheckoutStatus Status { get; private set; }

    public Money ItemsSubtotal { get; private set; } = default!;
    public Money ShippingTotal { get; private set; } = default!;
    public Money GrandTotal { get; private set; } = default!;

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public DateTime? SubmittedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }

    public IReadOnlyCollection<CheckoutLine> Lines => _lines.AsReadOnly();

    public static Checkout Start(
        CheckoutId id,
        CartId cartId,
        UserId userId,
        IReadOnlyCollection<CheckoutLineSnapshotData> snapshots,
        DateTime createdAtUtc)
    {
        if (snapshots is null || snapshots.Count == 0)
            throw new InvalidOperationException("Checkout cannot be started without item snapshots.");

        var lines = snapshots.Select(x => CheckoutLine.Create(
            CheckoutLineId.New(),
            x.OfferId,
            x.ProductId,
            x.SellerId,
            x.ProductTitle,
            x.Sku,
            x.Quantity,
            x.UnitPrice));

        return new Checkout(id, cartId, userId, lines, createdAtUtc);
    }

    public void SetLineRecipient(
        CheckoutLineId lineId,
        RecipientInfo recipient,
        DateTime nowUtc)
    {
        EnsureMutable();

        var line = GetRequiredLine(lineId);
        line.SetRecipient(recipient);

        UpdatedAtUtc = nowUtc;
    }

    public void SetLineShipping(
        CheckoutLineId lineId,
        ShippingSelection shipping,
        DateTime nowUtc)
    {
        EnsureMutable();

        var line = GetRequiredLine(lineId);
        line.SetShipping(shipping);

        UpdatedAtUtc = nowUtc;
        RecalculateTotals();
    }

    public void SetLinePayment(
        CheckoutLineId lineId,
        PaymentSelection payment,
        DateTime nowUtc)
    {
        EnsureMutable();

        var line = GetRequiredLine(lineId);
        line.SetPayment(payment);

        UpdatedAtUtc = nowUtc;
    }

    public bool CanSubmit()
    {
        if (Status != CheckoutStatus.Draft)
            return false;

        if (_lines.Count == 0)
            return false;

        return _lines.All(x => x.IsComplete());
    }

    public void Submit(DateTime nowUtc)
    {
        EnsureMutable();

        if (!CanSubmit())
            throw new InvalidOperationException("Checkout is not ready for submission.");

        Status = CheckoutStatus.Submitted;
        SubmittedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;

        AddDomainEvent(new CheckoutSubmittedDomainEvent(Id));
    }

    public void Cancel(DateTime nowUtc)
    {
        if (Status == CheckoutStatus.Submitted)
            throw new InvalidOperationException("Submitted checkout cannot be cancelled.");

        if (Status == CheckoutStatus.Cancelled)
            return;

        Status = CheckoutStatus.Cancelled;
        CancelledAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    private void RecalculateTotals()
    {
        var currency = _lines[0].UnitPrice.Currency;

        var itemsSubtotal = Money.Zero(currency);
        var shippingTotal = Money.Zero(currency);

        foreach (var line in _lines)
        {
            itemsSubtotal += line.LineTotal;

            if (line.Shipping is not null)
                shippingTotal += line.Shipping.Cost;
        }

        ItemsSubtotal = itemsSubtotal;
        ShippingTotal = shippingTotal;
        GrandTotal = ItemsSubtotal + ShippingTotal;
    }

    private CheckoutLine GetRequiredLine(CheckoutLineId lineId)
    {
        var line = _lines.FirstOrDefault(x => x.Id == lineId);
        if (line is null)
            throw new InvalidOperationException($"Checkout line '{lineId}' was not found.");

        return line;
    }

    private void EnsureMutable()
    {
        if (Status is CheckoutStatus.Submitted or CheckoutStatus.Cancelled or CheckoutStatus.Expired)
            throw new InvalidOperationException($"Checkout in status '{Status}' is not mutable.");
    }
}