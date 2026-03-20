using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.CancelPayment;

public sealed record CancelPaymentCommand(
    Guid PaymentId,
    string? ExternalStatus = null) : IRequest<Result>;