using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Payments;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.SetCheckoutLinePayment;

public sealed record SetCheckoutLinePaymentCommand(
    Guid CheckoutId,
    Guid LineId,
    PaymentProvider Provider,
    PaymentMethod Method,
    bool RequiresOnlineAuthorization) : IRequest<Result>;