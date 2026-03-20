using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Shipping;

namespace BazaR.Backend.Domain.ShippingProfiles;

public sealed class ShippingMethodConfig : Entity<ShippingMethodConfigId>
{
    public ShippingMethodType MethodType { get; private set; }
    public bool IsEnabled { get; private set; }

    public decimal BaseFee { get; private set; }
    public string Currency { get; private set; } = default!;
    public decimal? FreeShippingFromAmount { get; private set; }

    public bool AllowCashOnDelivery { get; private set; }

    public bool RequiresCity { get; private set; }
    public bool RequiresPickupPoint { get; private set; }
    public bool RequiresStreetAddress { get; private set; }

    public int? EstimatedDaysMin { get; private set; }
    public int? EstimatedDaysMax { get; private set; }

    public string? Title { get; private set; }
    public string? Description { get; private set; }

    private ShippingMethodConfig() { }

    private ShippingMethodConfig(
        ShippingMethodConfigId id,
        ShippingMethodType methodType,
        decimal baseFee,
        string currency,
        decimal? freeShippingFromAmount,
        bool allowCashOnDelivery,
        int? estimatedDaysMin,
        int? estimatedDaysMax,
        string? title,
        string? description,
        ShippingMethodRules rules) : base(id)
    {
        MethodType = methodType;
        IsEnabled = true;

        BaseFee = decimal.Round(baseFee, 2);
        Currency = currency.Trim().ToUpperInvariant();
        FreeShippingFromAmount = freeShippingFromAmount.HasValue
            ? decimal.Round(freeShippingFromAmount.Value, 2)
            : null;

        AllowCashOnDelivery = rules.SupportsCashOnDelivery && allowCashOnDelivery;

        RequiresCity = rules.RequiresCity;
        RequiresPickupPoint = rules.RequiresPickupPoint;
        RequiresStreetAddress = rules.RequiresStreetAddress;

        EstimatedDaysMin = estimatedDaysMin;
        EstimatedDaysMax = estimatedDaysMax;

        Title = Normalize(title);
        Description = Normalize(description);
    }

    public static Result<ShippingMethodConfig> Create(
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
        var validation = Validate(
            methodType,
            baseFee,
            currency,
            freeShippingFromAmount,
            estimatedDaysMin,
            estimatedDaysMax);

        if (validation.IsFailure)
            return Result<ShippingMethodConfig>.Failure(validation.Error);

        var rules = ShippingMethodRules.For(methodType);

        var config = new ShippingMethodConfig(
            ShippingMethodConfigId.New(),
            methodType,
            baseFee,
            currency,
            freeShippingFromAmount,
            allowCashOnDelivery,
            estimatedDaysMin,
            estimatedDaysMax,
            title,
            description,
            rules);

        return Result<ShippingMethodConfig>.Success(config);
    }

    public Result Update(
        decimal baseFee,
        string currency,
        decimal? freeShippingFromAmount,
        bool allowCashOnDelivery,
        int? estimatedDaysMin,
        int? estimatedDaysMax,
        string? title,
        string? description)
    {
        var validation = Validate(
            MethodType,
            baseFee,
            currency,
            freeShippingFromAmount,
            estimatedDaysMin,
            estimatedDaysMax);

        if (validation.IsFailure)
            return validation;

        var rules = ShippingMethodRules.For(MethodType);

        BaseFee = decimal.Round(baseFee, 2);
        Currency = currency.Trim().ToUpperInvariant();
        FreeShippingFromAmount = freeShippingFromAmount.HasValue
            ? decimal.Round(freeShippingFromAmount.Value, 2)
            : null;

        AllowCashOnDelivery = rules.SupportsCashOnDelivery && allowCashOnDelivery;

        RequiresCity = rules.RequiresCity;
        RequiresPickupPoint = rules.RequiresPickupPoint;
        RequiresStreetAddress = rules.RequiresStreetAddress;

        EstimatedDaysMin = estimatedDaysMin;
        EstimatedDaysMax = estimatedDaysMax;

        Title = Normalize(title);
        Description = Normalize(description);

        return Result.Success();
    }

    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;

    private static Result Validate(
        ShippingMethodType methodType,
        decimal baseFee,
        string currency,
        decimal? freeShippingFromAmount,
        int? estimatedDaysMin,
        int? estimatedDaysMax)
    {
        if (methodType == ShippingMethodType.Unknown)
            return Result.Failure(ShippingErrors.MethodTypeRequired);

        if (baseFee < 0)
            return Result.Failure(ShippingErrors.BaseFeeCannotBeNegative);

        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure(ShippingErrors.CurrencyRequired);

        if (freeShippingFromAmount.HasValue && freeShippingFromAmount.Value < 0)
            return Result.Failure(ShippingErrors.FreeShippingAmountInvalid);

        if (estimatedDaysMin.HasValue && estimatedDaysMin.Value < 0)
            return Result.Failure(ShippingErrors.EstimatedMinDaysInvalid);

        if (estimatedDaysMax.HasValue && estimatedDaysMax.Value < 0)
            return Result.Failure(ShippingErrors.EstimatedMaxDaysInvalid);

        if (estimatedDaysMin.HasValue &&
            estimatedDaysMax.HasValue &&
            estimatedDaysMin.Value > estimatedDaysMax.Value)
        {
            return Result.Failure(ShippingErrors.EstimatedRangeInvalid);
        }

        return Result.Success();
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}