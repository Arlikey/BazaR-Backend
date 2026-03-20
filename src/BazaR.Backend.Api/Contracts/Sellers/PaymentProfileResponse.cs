using BazaR.Backend.Api.Contracts.Sellers;

namespace BazaR.Backend.Api.Contracts.Sellers;

public sealed record PaymentProfileResponse(
    Guid Id,
    Guid SellerId,
    string Status,
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
    IReadOnlyCollection<PaymentMethodResponse> Methods);