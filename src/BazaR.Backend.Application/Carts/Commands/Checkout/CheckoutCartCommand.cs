using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.Checkout;

public sealed record CheckoutCartCommand(
    CheckoutCustomerDto Customer,
    CheckoutDeliveryDto Delivery,
    string? CustomerComment
) : IRequest<Result<Guid>>;

public sealed record CheckoutCustomerDto(
    string FirstName,
    string LastName,
    string Email,
    string? Phone);

public sealed record CheckoutDeliveryDto(
    DeliveryMethod Method,
    string City,
    string? Region,
    string? Warehouse,
    string? Street,
    string? Building,
    string? Apartment,
    string? PostalCode);