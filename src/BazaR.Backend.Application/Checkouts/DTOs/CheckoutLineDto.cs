using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Checkouts.DTOs
{
    public sealed record CheckoutLineDto(
     Guid Id,
     Guid ProductId,
     string ProductTitle,
     string Sku,
     int Quantity,

     decimal UnitPrice,
     decimal LineTotal,
     decimal ShippingCost,
     decimal GrandTotal,
     string Currency,

     string? RecipientName,
     string? RecipientPhone,
     string? RecipientEmail,

     string? ShippingMethod,
     string? ShippingCity,
     string? ShippingRegion,
     string? ShippingWarehouse,

     string? PaymentMethod,
     string? PaymentProvider,
     bool RequiresOnlineAuthorization
 );
}
