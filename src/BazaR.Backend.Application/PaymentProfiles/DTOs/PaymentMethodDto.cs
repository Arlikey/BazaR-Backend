using BazaR.Backend.Domain.PaymentProfiles;

namespace BazaR.Backend.Application.PaymentProfiles.DTOs;

public sealed record PaymentMethodDto(
    PaymentMethodType MethodType,
    bool IsEnabled,
    bool RequiresOnlineAuthorization,
    bool RequiresBankAccount,
    bool RequiresLiqPay,
    decimal? MinAmount,
    decimal? MaxAmount,
    string? Title,
    string? Description);