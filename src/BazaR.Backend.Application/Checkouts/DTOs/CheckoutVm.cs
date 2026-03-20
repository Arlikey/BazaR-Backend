using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Checkouts.DTOs
{
   
    public sealed record CheckoutVm(
        Guid CheckoutId,
        string Status,
        decimal ItemsSubtotal,
        decimal ShippingTotal,
        decimal GrandTotal,
        string Currency,
        IReadOnlyCollection<CheckoutLineVm> Lines);

    public sealed record CheckoutLineVm(
        Guid LineId,
        Guid OfferId,
        Guid ProductId,
        Guid SellerId,
        string ProductTitle,
        string Sku,
        int Quantity,
        decimal UnitPrice,
        decimal LineTotal,
        bool IsComplete,
        CheckoutRecipientVm? Recipient,
        CheckoutShippingVm? Shipping,
        CheckoutPaymentVm? Payment);

    public sealed record CheckoutRecipientVm(
        string FirstName,
        string LastName,
        string Phone,
        string? Email,
        bool IsCustomerRecipient);

    public sealed record CheckoutShippingVm(
        string MethodType,
        string Country,
        string Region,
        string City,
        string? Street,
        string? House,
        string? Apartment,
        string? PostalCode,
        string? PickupPointCode,
        string? PickupPointName,
        string? Comment,
        decimal Cost,
        string Currency);

    public sealed record CheckoutPaymentVm(
        string Provider,
        string Method,
        bool RequiresOnlineAuthorization);
}
