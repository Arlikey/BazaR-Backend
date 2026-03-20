using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.PaymentProfiles;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.AddPaymentMethod;

public sealed record AddPaymentMethodCommand(
    PaymentMethodType MethodType,
    bool RequiresOnlineAuthorization,
    bool RequiresBankAccount,
    bool RequiresLiqPay,
    decimal? MinAmount,
    decimal? MaxAmount,
    string? Title,
    string? Description) : IRequest<Result>;