using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Orders;

namespace BazaR.Backend.Api.Contracts.Customer.Carts;

public sealed record CheckoutCartRequest(
    Address DeliveryAddress,
    string? CustomerComment
);