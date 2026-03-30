using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Shippings;

public sealed class Shipping : AggregateRoot<ShippingId>
{
    private readonly List<ShippingParcel> _parcels = new();

    private Shipping() { }

    private Shipping(
        ShippingId id,
        OrderId orderId,
        UserId customerId,
        SellerId sellerId,
        ShippingMethod method,
        ShippingSettlementMode settlementMode,
        ShippingRecipient recipient,
        ShippingDestination destination,
        DateTimeOffset nowUtc) : base(id)
    {
        Id = id;
        OrderId = orderId;
        CustomerId = customerId;
        SellerId = sellerId;
        Method = method;
        SettlementMode = settlementMode;
        Recipient = recipient;
        Destination = destination;
        Status = ShippingStatus.AwaitingSender;
        CreatedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public OrderId OrderId { get; private set; } = default!;
    public UserId CustomerId { get; private set; } = default!;
    public SellerId SellerId { get; private set; } = default!;

    public ShippingMethod Method { get; private set; }
    public ShippingSettlementMode SettlementMode { get; private set; }
    public ShippingStatus Status { get; private set; }

    public ShippingSender? Sender { get; private set; }
    public ShippingRecipient Recipient { get; private set; } = default!;
    public ShippingDestination Destination { get; private set; } = default!;

    public IReadOnlyCollection<ShippingParcel> Parcels => _parcels.AsReadOnly();

    public string? TrackingNumber { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public DateTimeOffset? DispatchedAtUtc { get; private set; }
    public DateTimeOffset? ReadyForPickupAtUtc { get; private set; }
    public DateTimeOffset? DeliveredAtUtc { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    public bool IsNovaPoshta => Method == ShippingMethod.NovaPoshta;
    public bool IsBazaR => Method == ShippingMethod.BazaR;

    public static Result<Shipping> Create(
        ShippingId id,
        OrderId orderId,
        UserId customerId,
        SellerId sellerId,
        ShippingMethod method,
        ShippingSettlementMode settlementMode,
        ShippingRecipient recipient,
        ShippingDestination destination,
        DateTimeOffset? nowUtc = null)
    {
        if (id.Value == Guid.Empty)
            return Result<Shipping>.Failure(ShippingErrors.ShippingIdRequired);

        if (orderId.Value == Guid.Empty)
            return Result<Shipping>.Failure(ShippingErrors.OrderIdRequired);

        if (customerId.Value == Guid.Empty)
            return Result<Shipping>.Failure(ShippingErrors.CustomerIdRequired);

        if (sellerId.Value == Guid.Empty)
            return Result<Shipping>.Failure(ShippingErrors.SellerIdRequired);

        if (!Enum.IsDefined(method) || method == ShippingMethod.Unknown)
            return Result<Shipping>.Failure(ShippingErrors.MethodRequired);

        if (!Enum.IsDefined(settlementMode) || settlementMode == ShippingSettlementMode.None)
            return Result<Shipping>.Failure(ShippingErrors.SettlementModeRequired);

        if (recipient is null)
            return Result<Shipping>.Failure(ShippingErrors.RecipientRequired);

        if (destination is null)
            return Result<Shipping>.Failure(ShippingErrors.DestinationRequired);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        var shipping = new Shipping(
            id,
            orderId,
            customerId,
            sellerId,
            method,
            settlementMode,
            recipient,
            destination,
            now);

        return Result<Shipping>.Success(shipping);
    }

    public Result SetSender(
        ShippingSender sender,
        DateTimeOffset? nowUtc = null)
    {
        if (sender is null)
            return Result.Failure(ShippingErrors.SenderRequired);

        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        Sender = sender;
        UpdatedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;

        TryMoveToReadyToDispatch();

        return Result.Success();
    }

    public Result SetRecipient(
        ShippingRecipient recipient,
        DateTimeOffset? nowUtc = null)
    {
        if (recipient is null)
            return Result.Failure(ShippingErrors.RecipientRequired);

        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        Recipient = recipient;
        UpdatedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result SetDestination(
        ShippingDestination destination,
        DateTimeOffset? nowUtc = null)
    {
        if (destination is null)
            return Result.Failure(ShippingErrors.DestinationRequired);

        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        Destination = destination;
        UpdatedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;

        TryMoveToReadyToDispatch();

        return Result.Success();
    }

    public Result SetParcels(
        IReadOnlyCollection<ShippingParcel> parcels,
        DateTimeOffset? nowUtc = null)
    {
        if (parcels is null || parcels.Count == 0)
            return Result.Failure(ShippingErrors.ParcelsRequired);

        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (parcels.Any(x => x is null))
        {
            return Result.Failure(new Error(
                "Shipping.Parcels.Invalid",
                "Parcels collection contains null item."));
        }

        var duplicateRowNumbers = parcels
            .GroupBy(x => x.RowNumber)
            .Any(g => g.Count() > 1);

        if (duplicateRowNumbers)
        {
            return Result.Failure(new Error(
                "Shipping.Parcels.RowNumber.Duplicate",
                "Parcel row numbers must be unique."));
        }

        _parcels.Clear();
        _parcels.AddRange(parcels.OrderBy(x => x.RowNumber));

        UpdatedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;

        TryMoveToReadyToDispatch();

        return Result.Success();
    }

    public Result AddParcel(
        ShippingParcel parcel,
        DateTimeOffset? nowUtc = null)
    {
        if (parcel is null)
            return Result.Failure(ShippingErrors.ParcelRequired);

        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (_parcels.Any(x => x.RowNumber == parcel.RowNumber))
        {
            return Result.Failure(new Error(
                "Shipping.Parcel.RowNumber.Duplicate",
                "Parcel row number must be unique."));
        }

        _parcels.Add(parcel);
        SortParcels();

        UpdatedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;

        TryMoveToReadyToDispatch();

        return Result.Success();
    }

    public Result ClearParcels(DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        _parcels.Clear();
        UpdatedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;

        if (Status != ShippingStatus.AwaitingSender)
            Status = ShippingStatus.AwaitingSender;

        return Result.Success();
    }

    public Result SetSettlementMode(
        ShippingSettlementMode settlementMode,
        DateTimeOffset? nowUtc = null)
    {
        if (!Enum.IsDefined(settlementMode) || settlementMode == ShippingSettlementMode.None)
            return Result.Failure(ShippingErrors.SettlementModeRequired);

        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        SettlementMode = settlementMode;
        UpdatedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result MarkReadyToDispatch(DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        var validation = ValidateReadyToDispatch();
        if (validation.IsFailure)
            return validation;

        Status = ShippingStatus.ReadyToDispatch;
        UpdatedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result Dispatch(
        string trackingNumber,
        DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (Status != ShippingStatus.ReadyToDispatch)
            return Result.Failure(ShippingErrors.OnlyReadyToDispatchCanBeDispatched);

        if (string.IsNullOrWhiteSpace(trackingNumber))
            return Result.Failure(ShippingErrors.TrackingNumberRequired);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        TrackingNumber = trackingNumber.Trim();
        Status = ShippingStatus.Dispatched;
        DispatchedAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    public Result MarkReadyForPickup(DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (Status != ShippingStatus.Dispatched)
            return Result.Failure(ShippingErrors.OnlyDispatchedCanBeReadyForPickup);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = ShippingStatus.ReadyForPickup;
        ReadyForPickupAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    public Result MarkDelivered(DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (Status is not ShippingStatus.ReadyForPickup and not ShippingStatus.Dispatched)
            return Result.Failure(ShippingErrors.OnlyReadyForPickupOrDispatchedCanBeDelivered);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = ShippingStatus.Delivered;
        DeliveredAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    public Result Cancel(DateTimeOffset? nowUtc = null)
    {
        if (Status == ShippingStatus.Delivered)
            return Result.Failure(ShippingErrors.DeliveredCannotBeCancelled);

        if (Status == ShippingStatus.Cancelled)
            return Result.Success();

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = ShippingStatus.Cancelled;
        CancelledAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    private Result ValidateReadyToDispatch()
    {
        if (Sender is null)
            return Result.Failure(ShippingErrors.SenderRequired);

        if (_parcels.Count == 0)
            return Result.Failure(ShippingErrors.ParcelsRequired);

        if (IsNovaPoshta && string.IsNullOrWhiteSpace(Sender.PickupPointCode))
        {
            return Result.Failure(new Error(
                "Shipping.Sender.PickupPoint.Required",
                "Sender pickup point is required for Nova Poshta shipping."));
        }

        if (IsNovaPoshta && string.IsNullOrWhiteSpace(Destination.PickupPointCode))
        {
            return Result.Failure(new Error(
                "Shipping.Destination.PickupPoint.Required",
                "Recipient pickup point is required for Nova Poshta shipping."));
        }

        return Result.Success();
    }

    private void TryMoveToReadyToDispatch()
    {
        if (Status == ShippingStatus.Cancelled || Status == ShippingStatus.Delivered)
            return;

        var validation = ValidateReadyToDispatch();
        if (validation.IsSuccess)
            Status = ShippingStatus.ReadyToDispatch;
        else if (Status != ShippingStatus.Dispatched && Status != ShippingStatus.ReadyForPickup)
            Status = ShippingStatus.AwaitingSender;
    }

    private bool IsFinal()
        => Status is ShippingStatus.Delivered or ShippingStatus.Cancelled;

    private void SortParcels()
    {
        var ordered = _parcels.OrderBy(x => x.RowNumber).ToList();
        _parcels.Clear();
        _parcels.AddRange(ordered);
    }
}