using BazaR.Backend.Domain.Payments;

namespace BazaR.Backend.Api.Contracts.Payments;

public sealed record StartLiqPayCheckoutRequest(
    LiqPayPayType PayType,
    string Description);