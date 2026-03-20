using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Payments;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.StartLiqPayCheckout;

public sealed record StartLiqPayCheckoutCommand(
    Guid PaymentId,
    LiqPayPayType PayType,
    string Description) : IRequest<Result<LiqPayCheckoutStartResult>>;