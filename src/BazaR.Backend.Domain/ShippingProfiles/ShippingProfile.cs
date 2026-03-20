using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.ShippingProfiles;

namespace BazaR.Backend.Domain.Shipping;

public sealed class ShippingProfile : AggregateRoot<ShippingProfileId>
{
    private readonly List<ShippingMethodConfig> _methods = new();

    public SellerId SellerId { get; private set; } = default!;
    public ShippingProfileStatus Status { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<ShippingMethodConfig> Methods => _methods.AsReadOnly();

    private ShippingProfile() { }

    private ShippingProfile(
        ShippingProfileId id,
        SellerId sellerId,
        DateTimeOffset nowUtc) : base(id)
    {
        SellerId = sellerId;
        Status = ShippingProfileStatus.Draft;
        CreatedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public static Result<ShippingProfile> Create(
        SellerId sellerId,
        DateTimeOffset? nowUtc = null)
    {
        if (sellerId.Value == Guid.Empty)
            return Result<ShippingProfile>.Failure(ShippingErrors.SellerIdRequired);

        var now = nowUtc ?? DateTimeOffset.UtcNow;
        var profile = new ShippingProfile(ShippingProfileId.New(), sellerId, now);

        return Result<ShippingProfile>.Success(profile);
    }

    public Result AddMethod(
        ShippingMethodType methodType,
        decimal baseFee,
        string currency,
        decimal? freeShippingFromAmount,
        bool allowCashOnDelivery,
        int? estimatedDaysMin,
        int? estimatedDaysMax,
        string? title,
        string? description)
    {
        if (Status == ShippingProfileStatus.Archived)
            return Result.Failure(ShippingErrors.ArchivedProfileCannotBeModified);

        if (_methods.Any(x => x.MethodType == methodType))
            return Result.Failure(ShippingErrors.MethodAlreadyExists);

        var methodRes = ShippingMethodConfig.Create(
            methodType,
            baseFee,
            currency,
            freeShippingFromAmount,
            allowCashOnDelivery,
            estimatedDaysMin,
            estimatedDaysMax,
            title,
            description);

        if (methodRes.IsFailure)
            return Result.Failure(methodRes.Error);

        _methods.Add(methodRes.Value!);
        Touch();

        return Result.Success();
    }

    public Result UpdateMethod(
        ShippingMethodType methodType,
        decimal baseFee,
        string currency,
        decimal? freeShippingFromAmount,
        bool allowCashOnDelivery,
        int? estimatedDaysMin,
        int? estimatedDaysMax,
        string? title,
        string? description)
    {
        if (Status == ShippingProfileStatus.Archived)
            return Result.Failure(ShippingErrors.ArchivedProfileCannotBeModified);

        var method = _methods.FirstOrDefault(x => x.MethodType == methodType);
        if (method is null)
            return Result.Failure(ShippingErrors.MethodNotFound);

        var res = method.Update(
            baseFee,
            currency,
            freeShippingFromAmount,
            allowCashOnDelivery,
            estimatedDaysMin,
            estimatedDaysMax,
            title,
            description);

        if (res.IsFailure)
            return res;

        Touch();
        return Result.Success();
    }

    public Result EnableMethod(ShippingMethodType methodType)
    {
        if (Status == ShippingProfileStatus.Archived)
            return Result.Failure(ShippingErrors.ArchivedProfileCannotBeModified);

        var method = _methods.FirstOrDefault(x => x.MethodType == methodType);
        if (method is null)
            return Result.Failure(ShippingErrors.MethodNotFound);

        method.Enable();
        Touch();

        return Result.Success();
    }

    public Result DisableMethod(ShippingMethodType methodType)
    {
        if (Status == ShippingProfileStatus.Archived)
            return Result.Failure(ShippingErrors.ArchivedProfileCannotBeModified);

        var method = _methods.FirstOrDefault(x => x.MethodType == methodType);
        if (method is null)
            return Result.Failure(ShippingErrors.MethodNotFound);

        method.Disable();
        Touch();

        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == ShippingProfileStatus.Archived)
            return Result.Failure(ShippingErrors.ArchivedProfileCannotBeModified);

        if (_methods.Count == 0)
            return Result.Failure(ShippingErrors.ProfileMustHaveMethods);

        if (_methods.All(x => !x.IsEnabled))
            return Result.Failure(ShippingErrors.ProfileMustHaveEnabledMethods);

        Status = ShippingProfileStatus.Active;
        Touch();

        return Result.Success();
    }

    public Result Suspend()
    {
        if (Status == ShippingProfileStatus.Archived)
            return Result.Failure(ShippingErrors.ArchivedProfileCannotBeModified);

        Status = ShippingProfileStatus.Suspended;
        Touch();

        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == ShippingProfileStatus.Archived)
            return Result.Success();

        Status = ShippingProfileStatus.Archived;
        Touch();

        return Result.Success();
    }

    public ShippingMethodConfig? FindEnabledMethod(ShippingMethodType methodType)
        => _methods.FirstOrDefault(x => x.MethodType == methodType && x.IsEnabled);

    public IReadOnlyCollection<ShippingMethodConfig> GetEnabledMethods()
        => _methods.Where(x => x.IsEnabled).ToList();

    private void Touch()
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}