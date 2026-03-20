using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.RefundPayment;

public sealed record RefundPaymentCommand(
    Guid PaymentId,
    decimal Amount,
    string Currency) : IRequest<Result>;