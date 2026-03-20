using BazaR.Backend.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Payments.DTOs
{
    public sealed record LiqPayCheckoutRequest(
    string PublicKey,
    string PrivateKey,
    decimal Amount,
    string Currency,
    string Description,
    string OrderId,
    string ResultUrl,
    string ServerUrl,
    LiqPayPayType PayType);
}
