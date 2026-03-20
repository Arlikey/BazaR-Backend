using BazaR.Backend.Domain.Payments;

namespace BazaR.Backend.Api.Contracts.Customer
{
    public sealed record SetCheckoutLinePaymentRequest(
     PaymentMethod Method,
     PaymentProvider Provider,
     bool RequiresOnlineAuthorization);
}
