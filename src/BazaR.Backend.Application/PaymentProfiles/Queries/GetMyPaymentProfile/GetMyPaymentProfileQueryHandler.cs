using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.PaymentProfiles.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Queries.GetMyPaymentProfile;

public sealed class GetMyPaymentProfileQueryHandler
    : IRequestHandler<GetMyPaymentProfileQuery, Result<PaymentProfileDto>>
{
    private readonly IPaymentProfileRepository _profiles;
    private readonly ISellerRepository _sellers;
    private readonly ICurrentUser _current;

    public GetMyPaymentProfileQueryHandler(
        IPaymentProfileRepository profiles,
        ISellerRepository sellers,
        ICurrentUser current)
    {
        _profiles = profiles;
        _sellers = sellers;
        _current = current;
    }

    public async Task<Result<PaymentProfileDto>> Handle(GetMyPaymentProfileQuery request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<PaymentProfileDto>.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result<PaymentProfileDto>.Failure(new Error("Seller.NotFound", "Seller was not found."));

        var profile = await _profiles.GetBySellerIdAsync(seller.Id, ct);
        if (profile is null)
            return Result<PaymentProfileDto>.Failure(new Error("PaymentProfile.NotFound", "Payment profile was not found."));

        var dto = new PaymentProfileDto(
            profile.Id.Value,
            profile.SellerId.Value,
            profile.Status,
            profile.BankAccount?.RecipientName,
            profile.BankAccount?.Iban,
            profile.BankAccount?.BankName,
            profile.BankAccount?.TaxNumber,
            profile.BankAccount?.Swift,
            profile.BankAccount?.PurposeTemplate,
            profile.LiqPaySettings is not null,
            profile.LiqPaySettings?.PublicKey,
            profile.LiqPaySettings?.ResultUrl,
            profile.LiqPaySettings?.ServerCallbackUrl,
            profile.LiqPaySettings?.CheckoutEnabled,
            profile.LiqPaySettings?.PrivatPayEnabled,
            profile.LiqPaySettings?.InstallmentsEnabled,
            profile.CreatedAtUtc,
            profile.UpdatedAtUtc,
            profile.Methods
                .Select(x => new PaymentMethodDto(
                    x.MethodType,
                    x.IsEnabled,
                    x.RequiresOnlineAuthorization,
                    x.RequiresBankAccount,
                    x.RequiresLiqPay,
                    x.MinAmount,
                    x.MaxAmount,
                    x.Title,
                    x.Description))
                .ToList());

        return Result<PaymentProfileDto>.Success(dto);
    }
}