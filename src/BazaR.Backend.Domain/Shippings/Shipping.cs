using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.ShippingProfiles;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Shippings;

public sealed class Shipping : AggregateRoot<ShippingId>
{
    private Shipping() { }

    private Shipping(
        ShippingId id,
        OrderId orderId,
        UserId userId,
        SellerId sellerId,
        ShippingMethodType methodType,
        ShippingRecipient recipient,
        ShippingDestination destination,
        Money cost,
        bool cashOnDeliveryAllowed,
        string? comment,
        DateTimeOffset nowUtc) : base(id)
    {
        Id = id;
        OrderId = orderId;
        UserId = userId;
        SellerId = sellerId;
        MethodType = methodType;
        Recipient = recipient;
        Destination = destination;
        Cost = cost;
        CashOnDeliveryAllowed = cashOnDeliveryAllowed;
        Comment = Normalize(comment);

        Status = ShippingStatus.Pending;
        CreatedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public OrderId OrderId { get; private set; } = default!;
    public UserId UserId { get; private set; } = default!;
    public SellerId SellerId { get; private set; } = default!;

    public ShippingMethodType MethodType { get; private set; }
    public ShippingRecipient Recipient { get; private set; } = default!;
    public ShippingDestination Destination { get; private set; } = default!;
    public Money Cost { get; private set; } = default!;
    public bool CashOnDeliveryAllowed { get; private set; }
    public string? Comment { get; private set; }

    // Внутренние данные отправки
    public string? Carrier { get; private set; }
    public string? TrackingNumber { get; private set; }
    public string? TrackingUrl { get; private set; }

    // Для будущих интеграций
    public string? ExternalShipmentId { get; private set; }
    public string? ExternalStatusCode { get; private set; }
    public string? ExternalStatusName { get; private set; }

    public ShippingStatus Status { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public DateTimeOffset? PreparingAtUtc { get; private set; }
    public DateTimeOffset? ReadyToShipAtUtc { get; private set; }
    public DateTimeOffset? ShippedAtUtc { get; private set; }
    public DateTimeOffset? ArrivedAtPickupPointAtUtc { get; private set; }
    public DateTimeOffset? DeliveredAtUtc { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }
    public DateTimeOffset? ReturnedAtUtc { get; private set; }
    public DateTimeOffset? ExternalStatusUpdatedAtUtc { get; private set; }

    public bool IsPickup => MethodType == ShippingMethodType.PickupBazar;
    public bool IsNovaPoshta =>
        MethodType is ShippingMethodType.NovaPoshtaWarehouse
        or ShippingMethodType.NovaPoshtaLocker
        or ShippingMethodType.NovaPoshtaCourier;

    public static Result<Shipping> Create(
        ShippingId id,
        OrderId orderId,
        UserId userId,
        SellerId sellerId,
        ShippingMethodType methodType,
        ShippingRecipient recipient,
        ShippingDestination destination,
        Money cost,
        bool cashOnDeliveryAllowed,
        string? comment,
        DateTimeOffset? nowUtc = null)
    {
        if (id.Value == Guid.Empty)
            return Result<Shipping>.Failure(ShippingErrors.ShippingIdRequired);

        if (orderId.Value == Guid.Empty)
            return Result<Shipping>.Failure(ShippingErrors.OrderIdRequired);

        if (userId.Value == Guid.Empty)
            return Result<Shipping>.Failure(ShippingErrors.UserIdRequired);

        if (sellerId.Value == Guid.Empty)
            return Result<Shipping>.Failure(ShippingErrors.SellerIdRequired);

        if (methodType == ShippingMethodType.Unknown)
            return Result<Shipping>.Failure(ShippingErrors.MethodTypeRequired);

        if (recipient is null)
            return Result<Shipping>.Failure(ShippingErrors.RecipientRequired);

        if (destination is null)
            return Result<Shipping>.Failure(ShippingErrors.DestinationRequired);

        if (cost is null)
            return Result<Shipping>.Failure(ShippingErrors.CostRequired);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        var shipping = new Shipping(
            id,
            orderId,
            userId,
            sellerId,
            methodType,
            recipient,
            destination,
            cost,
            cashOnDeliveryAllowed,
            comment,
            now);

        return Result<Shipping>.Success(shipping);
    }

    public Result MarkPreparing(DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (Status != ShippingStatus.Pending)
            return Result.Failure(ShippingErrors.OnlyPendingCanBePrepared);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = ShippingStatus.Preparing;
        PreparingAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    public Result MarkReadyToShip(DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (Status != ShippingStatus.Preparing)
            return Result.Failure(ShippingErrors.OnlyPreparingCanBeReadyToShip);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = ShippingStatus.ReadyToShip;
        ReadyToShipAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

  
    public Result Ship(
        string carrier,
        string? trackingNumber,
        string? trackingUrl = null,
        string? externalShipmentId = null,
        DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (IsPickup)
            return Result.Failure(new Error(
                "Shipping.Pickup.CannotShip",
                "Pickup shipping cannot be marked as shipped. Use pickup flow instead."));

        if (Status is not ShippingStatus.Pending
            and not ShippingStatus.Preparing
            and not ShippingStatus.ReadyToShip)
        {
            return Result.Failure(ShippingErrors.InvalidStatusForShip);
        }

        if (string.IsNullOrWhiteSpace(carrier))
            return Result.Failure(ShippingErrors.CarrierRequired);

        if (string.IsNullOrWhiteSpace(trackingNumber))
            return Result.Failure(ShippingErrors.TrackingNumberRequired);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Carrier = carrier.Trim();
        TrackingNumber = trackingNumber.Trim();
        TrackingUrl = Normalize(trackingUrl);
        ExternalShipmentId = Normalize(externalShipmentId);

        Status = ShippingStatus.Shipped;
        ShippedAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    
    public Result AttachShipment(
        string carrier,
        string? trackingNumber,
        string? trackingUrl,
        string? externalShipmentId,
        DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (string.IsNullOrWhiteSpace(carrier))
            return Result.Failure(ShippingErrors.CarrierRequired);

        if (!IsPickup && string.IsNullOrWhiteSpace(trackingNumber))
            return Result.Failure(ShippingErrors.TrackingNumberRequired);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Carrier = carrier.Trim();
        TrackingNumber = Normalize(trackingNumber);
        TrackingUrl = Normalize(trackingUrl);
        ExternalShipmentId = Normalize(externalShipmentId);
        UpdatedAtUtc = now;

        return Result.Success();
    }

    
    public Result UpdateExternalStatus(
        string? externalStatusCode,
        string? externalStatusName,
        DateTimeOffset? nowUtc = null)
    {
        var now = nowUtc ?? DateTimeOffset.UtcNow;

        ExternalStatusCode = Normalize(externalStatusCode);
        ExternalStatusName = Normalize(externalStatusName);
        ExternalStatusUpdatedAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

   
    public Result MarkArrivedAtPickupPoint(DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (Status != ShippingStatus.Shipped)
            return Result.Failure(new Error(
                "Shipping.OnlyShippedCanArrive",
                "Only shipped shipping can arrive at pickup point."));

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        ArrivedAtPickupPointAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    /// <summary>
    /// Для обычной доставки.
    /// </summary>
    public Result MarkDelivered(DateTimeOffset? nowUtc = null)
    {
        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (IsPickup)
            return Result.Failure(new Error(
                "Shipping.Pickup.UseMarkPickedUp",
                "Pickup shipping must use MarkPickedUp."));

        if (Status != ShippingStatus.Shipped)
            return Result.Failure(ShippingErrors.OnlyShippedCanBeDelivered);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = ShippingStatus.Delivered;
        DeliveredAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    /// <summary>
    /// Для Baza-R самовывоза.
    /// </summary>
    public Result MarkPickedUp(DateTimeOffset? nowUtc = null)
    {
        if (!IsPickup)
            return Result.Failure(new Error(
                "Shipping.Pickup.InvalidMethod",
                "This shipping method is not pickup."));

        if (IsFinal())
            return Result.Failure(ShippingErrors.FinalStatusCannotBeChanged);

        if (Status is not ShippingStatus.Preparing and not ShippingStatus.ReadyToShip)
        {
            return Result.Failure(new Error(
                "Shipping.Pickup.NotReady",
                "Pickup order is not ready for handover."));
        }

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Carrier ??= "Baza-R Pickup";
        Status = ShippingStatus.Delivered;
        DeliveredAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    public Result Cancel(DateTimeOffset? nowUtc = null)
    {
        if (Status == ShippingStatus.Delivered)
            return Result.Failure(ShippingErrors.DeliveredCannotBeCancelled);

        if (Status == ShippingStatus.Returned)
            return Result.Failure(ShippingErrors.ReturnedCannotBeCancelled);

        if (Status == ShippingStatus.Cancelled)
            return Result.Success();

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = ShippingStatus.Cancelled;
        CancelledAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    public Result MarkReturned(DateTimeOffset? nowUtc = null)
    {
        if (Status is not ShippingStatus.Shipped and not ShippingStatus.Delivered)
            return Result.Failure(ShippingErrors.OnlyShippedOrDeliveredCanBeReturned);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = ShippingStatus.Returned;
        ReturnedAtUtc = now;
        UpdatedAtUtc = now;

        return Result.Success();
    }

    private bool IsFinal()
        => Status is ShippingStatus.Cancelled or ShippingStatus.Delivered or ShippingStatus.Returned;

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}