using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Payments;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BazaR.Backend.Infrastructure.Services;

public sealed class LiqPayCheckoutService : ILiqPayCheckoutService
{
    private const string DefaultActionUrl = "https://www.liqpay.ua/api/3/checkout";

    public LiqPayCheckoutPayload CreateCheckout(LiqPayCheckoutRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PublicKey))
            throw new InvalidOperationException("LiqPay public key is required.");

        if (string.IsNullOrWhiteSpace(request.PrivateKey))
            throw new InvalidOperationException("LiqPay private key is required.");

        if (request.Amount <= 0)
            throw new InvalidOperationException("LiqPay amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.Currency))
            throw new InvalidOperationException("LiqPay currency is required.");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new InvalidOperationException("LiqPay description is required.");

        if (string.IsNullOrWhiteSpace(request.OrderId))
            throw new InvalidOperationException("LiqPay order id is required.");

        var payTypes = MapPayType(request.PayType);

        var payload = new Dictionary<string, object?>
        {
            ["version"] = "3",
            ["public_key"] = request.PublicKey,
            ["action"] = "pay",
            ["amount"] = decimal.Round(request.Amount, 2),
            ["currency"] = request.Currency.Trim().ToUpperInvariant(),
            ["description"] = request.Description.Trim(),
            ["order_id"] = request.OrderId.Trim(),
            ["result_url"] = NormalizeNullable(request.ResultUrl),
            ["server_url"] = NormalizeNullable(request.ServerUrl),
            ["paytypes"] = payTypes,
            ["language"] = "uk"
        };

        var json = JsonSerializer.Serialize(payload);
        var data = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        var signature = CreateSignature(request.PrivateKey, data);

        return new LiqPayCheckoutPayload(
            ActionUrl: DefaultActionUrl,
            Data: data,
            Signature: signature,
            ExternalOrderReference: request.OrderId.Trim(),
            ExternalSessionId: null,
            ExternalStatus: null);
    }

    public LiqPayCallbackParseResult ParseAndValidateCallback(
        string data,
        string signature,
        string privateKey)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return InvalidResult(
                rawData: data,
                rawSignature: signature,
                merchantOrderReference: string.Empty,
                failureMessage: "Callback data is empty.");
        }

        if (string.IsNullOrWhiteSpace(signature))
        {
            return InvalidResult(
                rawData: data,
                rawSignature: signature,
                merchantOrderReference: string.Empty,
                failureMessage: "Callback signature is empty.");
        }

        if (string.IsNullOrWhiteSpace(privateKey))
        {
            return InvalidResult(
                rawData: data,
                rawSignature: signature,
                merchantOrderReference: string.Empty,
                failureMessage: "Private key is empty.");
        }

        string json;
        try
        {
            json = Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }
        catch
        {
            return InvalidResult(
                rawData: data,
                rawSignature: signature,
                merchantOrderReference: string.Empty,
                failureMessage: "Callback data is not valid base64.");
        }

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(json);
        }
        catch
        {
            return InvalidResult(
                rawData: data,
                rawSignature: signature,
                merchantOrderReference: string.Empty,
                failureMessage: "Callback data is not valid JSON.");
        }

        using (doc)
        {
            var expectedSignature = CreateSignature(privateKey, data);
            var isValid = string.Equals(
                expectedSignature,
                signature.Trim(),
                StringComparison.Ordinal);

            var root = doc.RootElement;

            var merchantOrderReference = GetString(root, "order_id") ?? string.Empty;
            var externalPaymentId =
                GetString(root, "payment_id") ??
                GetString(root, "liqpay_order_id");

            var externalTransactionId =
                GetString(root, "transaction_id") ??
                GetString(root, "tran_id");

            var externalStatus = GetString(root, "status");
            var actualPayType = ParsePayType(GetString(root, "paytype"));
            var providerAmount = GetDecimal(root, "amount");
            var providerCurrency = GetString(root, "currency");
            var cardMask = GetString(root, "sender_card_mask2");
            var cardBank = GetString(root, "sender_card_bank");
            var cardType = GetString(root, "card_type");

            var failureCode =
                GetString(root, "err_code") ??
                GetString(root, "code");

            var failureMessage =
                GetString(root, "err_description") ??
                GetString(root, "failure_reason") ??
                GetString(root, "description");

            return new LiqPayCallbackParseResult(
                IsValid: isValid,
                MerchantOrderReference: merchantOrderReference,
                ExternalPaymentId: NormalizeNullable(externalPaymentId),
                ExternalTransactionId: NormalizeNullable(externalTransactionId),
                ExternalStatus: NormalizeNullable(externalStatus),
                ActualPayType: actualPayType,
                ProviderAmount: providerAmount,
                ProviderCurrency: NormalizeNullable(providerCurrency),
                CardMask: NormalizeNullable(cardMask),
                CardBank: NormalizeNullable(cardBank),
                CardType: NormalizeNullable(cardType),
                FailureCode: NormalizeNullable(failureCode),
                FailureMessage: NormalizeNullable(failureMessage),
                RawData: data,
                RawSignature: signature);
        }
    }

    private static string MapPayType(LiqPayPayType payType)
    {
        return payType switch
        {
            LiqPayPayType.Card => "card",
            LiqPayPayType.PrivatPay => "privat24",
            LiqPayPayType.Installments => "installments",
            _ => "card"
        };
    }

    private static LiqPayPayType? ParsePayType(string? value)
    {
        var normalized = NormalizeNullable(value)?.ToLowerInvariant();

        return normalized switch
        {
            "card" => LiqPayPayType.Card,
            "privat24" => LiqPayPayType.PrivatPay,
            "privatpay" => LiqPayPayType.PrivatPay,
            "installments" => LiqPayPayType.Installments,
            "parts" => LiqPayPayType.Installments,
            _ => null
        };
    }

    private static string CreateSignature(string privateKey, string data)
    {
        var raw = privateKey + data + privateKey;
        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = SHA1.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    private static string? GetString(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var prop))
            return null;

        return prop.ValueKind switch
        {
            JsonValueKind.String => prop.GetString(),
            JsonValueKind.Number => prop.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => null
        };
    }

    private static decimal? GetDecimal(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var prop))
            return null;

        if (prop.ValueKind == JsonValueKind.Number && prop.TryGetDecimal(out var d))
            return d;

        if (prop.ValueKind == JsonValueKind.String &&
            decimal.TryParse(prop.GetString(), out var parsed))
            return parsed;

        return null;
    }

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static LiqPayCallbackParseResult InvalidResult(
        string rawData,
        string rawSignature,
        string merchantOrderReference,
        string? failureMessage)
    {
        return new LiqPayCallbackParseResult(
            IsValid: false,
            MerchantOrderReference: merchantOrderReference,
            ExternalPaymentId: null,
            ExternalTransactionId: null,
            ExternalStatus: null,
            ActualPayType: null,
            ProviderAmount: null,
            ProviderCurrency: null,
            CardMask: null,
            CardBank: null,
            CardType: null,
            FailureCode: null,
            FailureMessage: failureMessage,
            RawData: rawData,
            RawSignature: rawSignature);
    }
}