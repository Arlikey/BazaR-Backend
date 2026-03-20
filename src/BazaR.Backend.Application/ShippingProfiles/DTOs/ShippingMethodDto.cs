using BazaR.Backend.Domain.ShippingProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.ShippingProfiles.DTOs
{
    public sealed record ShippingMethodDto(
     ShippingMethodType MethodType,
     bool IsEnabled,
     decimal BaseFee,
     string Currency,
     decimal? FreeShippingFromAmount,
     bool AllowCashOnDelivery,
     bool RequiresCity,
     bool RequiresPickupPoint,
     bool RequiresStreetAddress,
     int? EstimatedDaysMin,
     int? EstimatedDaysMax,
     string? Title,
     string? Description);
}
