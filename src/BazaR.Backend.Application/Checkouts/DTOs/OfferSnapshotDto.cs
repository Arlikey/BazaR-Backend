using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Checkouts.DTOs
{
    public sealed record OfferSnapshotDto(
    Guid OfferId,
    Guid ProductId,
    Guid SellerId,
    string ProductTitle,
    string Sku);
}
