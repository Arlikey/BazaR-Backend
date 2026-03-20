using BazaR.Backend.Domain.ShippingProfiles;

namespace BazaR.Backend.Api.Contracts.Customer
{
    public sealed record SetCheckoutLineShippingRequest(
     ShippingMethodType Method,
     string Country,
     string Region,
     string City,
     string? Street,
     string? House,
     string? Apartment,
     string? PostalCode,
     string? WarehouseCode,
     string? WarehouseName,
     string? Comment);
}
