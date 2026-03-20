using BazaR.Backend.Domain.PaymentProfiles;

namespace BazaR.Backend.Application.PaymentProfiles.DTOs;

public sealed record PaymentProfileDto(
    Guid Id,
    Guid SellerId,
    PaymentProfileStatus Status,
    string? BankRecipientName,
    string? BankIban,
    string? BankName,
    string? BankTaxNumber,
    string? BankSwift,
    string? BankPurposeTemplate,
    bool HasLiqPaySettings,
    string? LiqPayPublicKey,
    string? LiqPayResultUrl,
    string? LiqPayServerCallbackUrl,
    bool? LiqPayCheckoutEnabled,
    bool? LiqPayPrivatPayEnabled,
    bool? LiqPayInstallmentsEnabled,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyCollection<PaymentMethodDto> Methods);