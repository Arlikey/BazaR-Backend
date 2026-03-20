using BazaR.Backend.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.ShippingProfiles.DTOs
{
    public sealed record ShippingProfileDto(
     Guid Id,
     Guid SellerId,
     ShippingProfileStatus Status,
     DateTimeOffset CreatedAtUtc,
     DateTimeOffset UpdatedAtUtc,
     IReadOnlyCollection<ShippingMethodDto> Methods);
}
