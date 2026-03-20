using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Application.Abstractions.Services;

public interface IOrderFactory
{
    Result<Order> CreateFromCheckoutLine(
        Checkout checkout,
        CheckoutLine line,
        UserId userId,
        DateTimeOffset nowUtc);
}