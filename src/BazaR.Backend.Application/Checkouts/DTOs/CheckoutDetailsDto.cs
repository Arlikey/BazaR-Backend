using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Checkouts.DTOs
{
    public sealed record CheckoutDetailsDto(
    Guid Id,
    int Status,

    decimal ItemsSubtotal,
    decimal ShippingTotal,
    decimal GrandTotal,
    string Currency,

    IReadOnlyList<CheckoutLineDto> Lines
);
}
