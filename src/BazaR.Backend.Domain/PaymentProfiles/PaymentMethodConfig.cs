using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.PaymentProfiles;

public sealed class PaymentMethodConfig : Entity<PaymentMethodConfigId>
{
    public PaymentMethodType MethodType { get; private set; }
    public bool IsEnabled { get; private set; }

    public bool RequiresOnlineAuthorization { get; private set; }
    public bool RequiresBankAccount { get; private set; }
    public bool RequiresLiqPay { get; private set; }

    public decimal? MinAmount { get; private set; }
    public decimal? MaxAmount { get; private set; }

    public string? Title { get; private set; }
    public string? Description { get; private set; }

    private PaymentMethodConfig() { }

    private PaymentMethodConfig(
        PaymentMethodConfigId id,
        PaymentMethodType methodType,
        bool requiresOnlineAuthorization,
        bool requiresBankAccount,
        bool requiresLiqPay,
        decimal? minAmount,
        decimal? maxAmount,
        string? title,
        string? description) : base(id)
    {
        MethodType = methodType;
        IsEnabled = true;
        RequiresOnlineAuthorization = requiresOnlineAuthorization;
        RequiresBankAccount = requiresBankAccount;
        RequiresLiqPay = requiresLiqPay;
        MinAmount = minAmount;
        MaxAmount = maxAmount;
        Title = Normalize(title);
        Description = Normalize(description);
    }

    public static Result<PaymentMethodConfig> Create(
        PaymentMethodType methodType,
        bool requiresOnlineAuthorization,
        bool requiresBankAccount,
        bool requiresLiqPay,
        decimal? minAmount,
        decimal? maxAmount,
        string? title,
        string? description)
    {
        if (methodType == PaymentMethodType.Unknown)
            return Result<PaymentMethodConfig>.Failure(PaymentProfileErrors.MethodTypeRequired);

        if (minAmount.HasValue && minAmount.Value < 0)
            return Result<PaymentMethodConfig>.Failure(PaymentProfileErrors.MinAmountInvalid);

        if (maxAmount.HasValue && maxAmount.Value < 0)
            return Result<PaymentMethodConfig>.Failure(PaymentProfileErrors.MaxAmountInvalid);

        if (minAmount.HasValue && maxAmount.HasValue && minAmount.Value > maxAmount.Value)
            return Result<PaymentMethodConfig>.Failure(PaymentProfileErrors.AmountRangeInvalid);

        return Result<PaymentMethodConfig>.Success(new PaymentMethodConfig(
            PaymentMethodConfigId.New(),
            methodType,
            requiresOnlineAuthorization,
            requiresBankAccount,
            requiresLiqPay,
            minAmount,
            maxAmount,
            title,
            description));
    }

    public Result Update(
        bool requiresOnlineAuthorization,
        bool requiresBankAccount,
        bool requiresLiqPay,
        decimal? minAmount,
        decimal? maxAmount,
        string? title,
        string? description)
    {
        if (minAmount.HasValue && minAmount.Value < 0)
            return Result.Failure(PaymentProfileErrors.MinAmountInvalid);

        if (maxAmount.HasValue && maxAmount.Value < 0)
            return Result.Failure(PaymentProfileErrors.MaxAmountInvalid);

        if (minAmount.HasValue && maxAmount.HasValue && minAmount.Value > maxAmount.Value)
            return Result.Failure(PaymentProfileErrors.AmountRangeInvalid);

        RequiresOnlineAuthorization = requiresOnlineAuthorization;
        RequiresBankAccount = requiresBankAccount;
        RequiresLiqPay = requiresLiqPay;
        MinAmount = minAmount;
        MaxAmount = maxAmount;
        Title = Normalize(title);
        Description = Normalize(description);

        return Result.Success();
    }

    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}