using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Payments.DTOs
{
    public sealed record LiqPayCheckoutPayload(
    string ActionUrl,
    string Data,
    string Signature,
    string ExternalOrderReference,
    string? ExternalSessionId,
    string? ExternalStatus);
}
