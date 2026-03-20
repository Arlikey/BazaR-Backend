using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.PaymentProfiles;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.DisablePaymentMethod;

public sealed record DisablePaymentMethodCommand(
    PaymentMethodType MethodType) : IRequest<Result>;