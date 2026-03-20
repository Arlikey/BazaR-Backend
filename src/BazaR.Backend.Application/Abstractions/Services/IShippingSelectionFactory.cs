using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Shipping;
using BazaR.Backend.Domain.ShippingProfiles;

namespace BazaR.Backend.Application.Abstractions.Services;

public interface IShippingSelectionFactory
{
    Result<ShippingSelection> Create(
        ShippingProfile profile,
        ShippingMethodType methodType,
        string country,
        string region,
        string city,
        string? street,
        string? house,
        string? apartment,
        string? postalCode,
        string? pickupPointCode,
        string? pickupPointName,
        string? comment);
}