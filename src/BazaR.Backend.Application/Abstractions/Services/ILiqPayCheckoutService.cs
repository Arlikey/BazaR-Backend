using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Payments;

namespace BazaR.Backend.Application.Abstractions.Services;

public interface ILiqPayCheckoutService
{
    LiqPayCheckoutPayload CreateCheckout(LiqPayCheckoutRequest request);

    LiqPayCallbackParseResult ParseAndValidateCallback(
        string data,
        string signature,
        string privateKey);
}