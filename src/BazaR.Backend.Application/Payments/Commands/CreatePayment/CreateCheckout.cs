using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Payments;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid OrderId,
    Guid SellerId,
    Guid UserId,
    PaymentProvider Provider,
    PaymentMethod Method,
    decimal Amount,
    string Currency,
    string MerchantOrderReference) : IRequest<Result<Guid>>;