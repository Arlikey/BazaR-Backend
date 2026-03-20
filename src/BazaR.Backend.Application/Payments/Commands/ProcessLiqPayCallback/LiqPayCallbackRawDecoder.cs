using System.Text;
using System.Text.Json;

namespace BazaR.Backend.Application.Payments.Commands.ProcessLiqPayCallback;

internal static class LiqPayCallbackRawDecoder
{
    public static string? TryExtractMerchantOrderReference(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return null;

        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("order_id", out var orderId))
                return orderId.GetString();

            return null;
        }
        catch
        {
            return null;
        }
    }
}