using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Shippings.DTOs.NovaPoshta
{
    public sealed record NovaPoshtaCreateShipmentResult(
     bool IsSuccess,
     string? TrackingNumber,
     string? ExternalShipmentId,
     string? TrackingUrl,
     string? RawStatusCode,
     string? RawStatusName,
     string? ErrorCode,
     string? ErrorMessage);
}
