using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.PaymentProfiles;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.UpdatePaymentMethod;

public sealed record UpdatePaymentMethodCommand(
    PaymentMethodType MethodType,
    bool RequiresOnlineAuthorization,
    bool RequiresBankAccount,
    bool RequiresLiqPay,
    decimal? MinAmount,
    decimal? MaxAmount,
    string? Title,
    string? Description) : IRequest<Result>;