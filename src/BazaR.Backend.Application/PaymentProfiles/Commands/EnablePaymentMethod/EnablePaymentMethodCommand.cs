using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.PaymentProfiles;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.EnablePaymentMethod;

public sealed record EnablePaymentMethodCommand(
    PaymentMethodType MethodType) : IRequest<Result>;