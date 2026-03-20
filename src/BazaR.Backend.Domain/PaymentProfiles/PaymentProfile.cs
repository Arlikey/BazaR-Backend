using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Domain.PaymentProfiles;

public sealed class PaymentProfile : AggregateRoot<PaymentProfileId>
{
    private readonly List<PaymentMethodConfig> _methods = new();

    public SellerId SellerId { get; private set; } = default!;
    public PaymentProfileStatus Status { get; private set; }

    public BankAccount? BankAccount { get; private set; }
    public LiqPaySettings? LiqPaySettings { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<PaymentMethodConfig> Methods => _methods.AsReadOnly();

    private PaymentProfile() { }

    private PaymentProfile(
        PaymentProfileId id,
        SellerId sellerId,
        DateTimeOffset nowUtc) : base(id)
    {
        SellerId = sellerId;
        Status = PaymentProfileStatus.Draft;
        CreatedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public static Result<PaymentProfile> Create(
        SellerId sellerId,
        DateTimeOffset? nowUtc = null)
    {
        if (sellerId.Value == Guid.Empty)
            return Result<PaymentProfile>.Failure(PaymentProfileErrors.SellerIdRequired);

        var now = nowUtc ?? DateTimeOffset.UtcNow;
        return Result<PaymentProfile>.Success(new PaymentProfile(PaymentProfileId.New(), sellerId, now));
    }

    public Result SetBankAccount(BankAccount bankAccount)
    {
        if (Status == PaymentProfileStatus.Archived)
            return Result.Failure(PaymentProfileErrors.ArchivedProfileCannotBeModified);

        if (bankAccount is null)
            throw new InvalidOperationException("Bank account is required.");

        BankAccount = bankAccount;
        Touch();

        return Result.Success();
    }

    public Result SetLiqPaySettings(LiqPaySettings liqPaySettings)
    {
        if (Status == PaymentProfileStatus.Archived)
            return Result.Failure(PaymentProfileErrors.ArchivedProfileCannotBeModified);

        if (liqPaySettings is null)
            throw new InvalidOperationException("LiqPay settings are required.");

        LiqPaySettings = liqPaySettings;
        Touch();

        return Result.Success();
    }

    public Result AddMethod(
        PaymentMethodType methodType,
        bool requiresOnlineAuthorization,
        bool requiresBankAccount,
        bool requiresLiqPay,
        decimal? minAmount,
        decimal? maxAmount,
        string? title,
        string? description)
    {
        if (Status == PaymentProfileStatus.Archived)
            return Result.Failure(PaymentProfileErrors.ArchivedProfileCannotBeModified);

        if (_methods.Any(x => x.MethodType == methodType))
            return Result.Failure(PaymentProfileErrors.MethodAlreadyExists);

        var methodResult = PaymentMethodConfig.Create(
            methodType,
            requiresOnlineAuthorization,
            requiresBankAccount,
            requiresLiqPay,
            minAmount,
            maxAmount,
            title,
            description);

        if (methodResult.IsFailure)
            return Result.Failure(methodResult.Error);

        _methods.Add(methodResult.Value!);
        Touch();

        return Result.Success();
    }

    public Result UpdateMethod(
        PaymentMethodType methodType,
        bool requiresOnlineAuthorization,
        bool requiresBankAccount,
        bool requiresLiqPay,
        decimal? minAmount,
        decimal? maxAmount,
        string? title,
        string? description)
    {
        if (Status == PaymentProfileStatus.Archived)
            return Result.Failure(PaymentProfileErrors.ArchivedProfileCannotBeModified);

        var method = _methods.FirstOrDefault(x => x.MethodType == methodType);
        if (method is null)
            return Result.Failure(PaymentProfileErrors.MethodNotFound);

        var result = method.Update(
            requiresOnlineAuthorization,
            requiresBankAccount,
            requiresLiqPay,
            minAmount,
            maxAmount,
            title,
            description);

        if (result.IsFailure)
            return result;

        Touch();
        return Result.Success();
    }

    public Result EnableMethod(PaymentMethodType methodType)
    {
        if (Status == PaymentProfileStatus.Archived)
            return Result.Failure(PaymentProfileErrors.ArchivedProfileCannotBeModified);

        var method = _methods.FirstOrDefault(x => x.MethodType == methodType);
        if (method is null)
            return Result.Failure(PaymentProfileErrors.MethodNotFound);

        method.Enable();
        Touch();

        return Result.Success();
    }

    public Result DisableMethod(PaymentMethodType methodType)
    {
        if (Status == PaymentProfileStatus.Archived)
            return Result.Failure(PaymentProfileErrors.ArchivedProfileCannotBeModified);

        var method = _methods.FirstOrDefault(x => x.MethodType == methodType);
        if (method is null)
            return Result.Failure(PaymentProfileErrors.MethodNotFound);

        method.Disable();
        Touch();

        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == PaymentProfileStatus.Archived)
            return Result.Failure(PaymentProfileErrors.ArchivedProfileCannotBeModified);

        if (_methods.Count == 0)
            return Result.Failure(PaymentProfileErrors.ProfileMustHaveMethods);

        if (_methods.All(x => !x.IsEnabled))
            return Result.Failure(PaymentProfileErrors.ProfileMustHaveEnabledMethods);

        foreach (var method in _methods.Where(x => x.IsEnabled))
        {
            if (method.RequiresBankAccount && BankAccount is null)
                return Result.Failure(PaymentProfileErrors.BankAccountRequired);

            if (method.RequiresLiqPay && LiqPaySettings is null)
                return Result.Failure(PaymentProfileErrors.LiqPaySettingsRequired);
        }

        Status = PaymentProfileStatus.Active;
        Touch();

        return Result.Success();
    }

    public Result Suspend()
    {
        if (Status == PaymentProfileStatus.Archived)
            return Result.Failure(PaymentProfileErrors.ArchivedProfileCannotBeModified);

        Status = PaymentProfileStatus.Suspended;
        Touch();

        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == PaymentProfileStatus.Archived)
            return Result.Success();

        Status = PaymentProfileStatus.Archived;
        Touch();

        return Result.Success();
    }

    public PaymentMethodConfig? FindEnabledMethod(PaymentMethodType methodType)
        => _methods.FirstOrDefault(x => x.MethodType == methodType && x.IsEnabled);

    private void Touch()
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}