using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Shippings.DTOs.NovaPoshta
{
    public sealed record NovaPoshtaTrackShipmentResult(
    bool IsSuccess,
    string? TrackingNumber,
    string? RawStatusCode,
    string? RawStatusName,
    bool IsCreated,
    bool IsInTransit,
    bool IsArrivedAtPickupPoint,
    bool IsDelivered,
    string? ErrorCode,
    string? ErrorMessage);
}
