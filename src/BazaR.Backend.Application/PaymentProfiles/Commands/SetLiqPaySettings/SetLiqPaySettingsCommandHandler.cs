using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.PaymentProfiles;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.SetLiqPaySettings;

public sealed class SetLiqPaySettingsCommandHandler
    : IRequestHandler<SetLiqPaySettingsCommand, Result>
{
    private readonly IPaymentProfileRepository _profiles;
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SetLiqPaySettingsCommandHandler(
        IPaymentProfileRepository profiles,
        ISellerRepository sellers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _profiles = profiles;
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(SetLiqPaySettingsCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller was not found."));

        var profile = await _profiles.GetBySellerIdAsync(seller.Id, ct);
        if (profile is null)
            return Result.Failure(new Error("PaymentProfile.NotFound", "Payment profile was not found."));

        var liqPayResult = LiqPaySettings.Create(
            request.PublicKey,
            request.PrivateKey,
            request.ResultUrl,
            request.ServerCallbackUrl,
            request.CheckoutEnabled,
            request.PrivatPayEnabled,
            request.InstallmentsEnabled);

        if (liqPayResult.IsFailure)
            return Result.Failure(liqPayResult.Error);

        var result = profile.SetLiqPaySettings(liqPayResult.Value!);
        if (result.IsFailure)
            return result;

        _profiles.Update(profile);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}