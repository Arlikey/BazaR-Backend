using BazaR.Backend.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Payments.DTOs
{
    public sealed record LiqPayCallbackParseResult(
     bool IsValid,
     string MerchantOrderReference,
     string? ExternalPaymentId,
     string? ExternalTransactionId,
     string? ExternalStatus,
     LiqPayPayType? ActualPayType,
     decimal? ProviderAmount,
     string? ProviderCurrency,
     string? CardMask,
     string? CardBank,
     string? CardType,
     string? FailureCode,
     string? FailureMessage,
     string RawData,
     string RawSignature);
}
