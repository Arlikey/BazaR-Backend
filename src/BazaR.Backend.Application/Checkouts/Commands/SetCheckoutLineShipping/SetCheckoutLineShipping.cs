using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.ShippingProfiles;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.SetCheckoutLineShipping;

public sealed record SetCheckoutLineShippingCommand(
    Guid CheckoutId,
    Guid LineId,
    ShippingMethodType Method,
    string? Country,
    string? Region,
    string? City,
    string? Street,
    string? House,
    string? Apartment,
    string? PostalCode,
    string? WarehouseCode,
    string? WarehouseName,
    string? Comment) : IRequest<Result>;