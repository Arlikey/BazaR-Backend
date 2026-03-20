using BazaR.Backend.Domain.Payments;

namespace BazaR.Backend.Domain.Checkouts;

public sealed record PaymentSelection(
    PaymentMethod Method,
    string? Provider,
    bool RequiresOnlineAuthorization)
{
    public static PaymentSelection Create(
        PaymentMethod method,
        string? provider,
        bool requiresOnlineAuthorization)
    {
        if (method == PaymentMethod.Unknown)
            throw new InvalidOperationException("Payment method is required.");

        return new PaymentSelection(
            method,
            string.IsNullOrWhiteSpace(provider) ? null : provider.Trim(),
            requiresOnlineAuthorization);
    }
}