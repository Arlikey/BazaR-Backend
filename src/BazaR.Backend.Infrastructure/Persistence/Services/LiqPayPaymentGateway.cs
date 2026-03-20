/*

namespace BazaR.Backend.Infrastructure.Persistence.Services;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Payments;
using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Payments;
using BazaR.Backend.Infrastructure.Payments;
using Microsoft.Extensions.Options;

public sealed class LiqPayPaymentGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly LiqPayOptions _options;

    public LiqPayPaymentGateway(
        HttpClient httpClient,
        IOptions<LiqPayOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public Task<CreateCheckoutResult> CreateCheckoutAsync(
        CreateCheckoutRequest request,
        CancellationToken ct)
    {
        var payload = new Dictionary<string, object?>
        {
            ["public_key"] = _options.PublicKey,
            ["version"] = 3,
            ["action"] = "pay",
            ["amount"] = request.Amount,
            ["currency"] = request.Currency.ToUpperInvariant(),
            ["description"] = request.Description,
            ["order_id"] = request.MerchantOrderReference,
            ["result_url"] = request.ResultUrl,
            ["server_url"] = request.ServerUrl
        };

        if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
            payload["customer"] = request.CustomerEmail;

        var data = EncodePayload(payload);
        var signature = Sign(data);

        return Task.FromResult(new CreateCheckoutResult(
            CheckoutActionUrl: _options.CheckoutUrl,
            Data: data,
            Signature: signature,
            ExternalOrderReference: request.MerchantOrderReference,
            ExternalSessionId: null,
            ExternalStatus: "created"));
    }

    public async Task<PaymentStatusCheckResult> GetStatusAsync(
        PaymentStatusCheckRequest request,
        CancellationToken ct)
    {
        var payload = new Dictionary<string, object?>
        {
            ["public_key"] = _options.PublicKey,
            ["version"] = 3,
            ["action"] = "status",
            ["order_id"] = request.MerchantOrderReference
        };

        var response = await SendApiRequestAsync(payload, ct);

        var status = GetString(response, "status");
        var transactionId = GetString(response, "transaction_id");

        return new PaymentStatusCheckResult(
            ExternalPaymentId: transactionId,
            ExternalStatus: status,
            IsAuthorized: IsAuthorizedStatus(status),
            IsPaid: IsPaidStatus(status),
            IsFailed: IsFailedStatus(status),
            IsCancelled: IsCancelledStatus(status));
    }

    public Task<RefundPaymentResult> RefundAsync(
        RefundPaymentRequest request,
        CancellationToken ct)
    {
        throw new NotImplementedException("Refund flow can be added after pay + callback + status is finished.");
    }

    public Task<ParsedPaymentCallback> ParseCallbackAsync(
        string payload,
        IReadOnlyDictionary<string, string> headers,
        CancellationToken ct)
    {
        var form = ParseFormEncoded(payload);

        if (!form.TryGetValue("data", out var data) || string.IsNullOrWhiteSpace(data))
            throw new InvalidOperationException("LiqPay callback missing data.");

        if (!form.TryGetValue("signature", out var signature) || string.IsNullOrWhiteSpace(signature))
            throw new InvalidOperationException("LiqPay callback missing signature.");

        var expectedSignature = Sign(data);
        if (!FixedTimeEquals(signature, expectedSignature))
            throw new InvalidOperationException("Invalid LiqPay callback signature.");

        var json = DecodePayload(data);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var status = GetString(root, "status");
        var orderId = GetString(root, "order_id");
        var transactionId = GetString(root, "transaction_id");

        if (string.IsNullOrWhiteSpace(orderId))
            throw new InvalidOperationException("LiqPay callback missing order_id.");

        return Task.FromResult(new ParsedPaymentCallback(
            Provider: PaymentProvider.LiqPay,
            MerchantOrderReference: orderId,
            ExternalPaymentId: transactionId,
            ExternalOrderReference: orderId,
            ExternalStatus: status,
            IsAuthorized: IsAuthorizedStatus(status),
            IsPaid: IsPaidStatus(status),
            IsFailed: IsFailedStatus(status),
            IsCancelled: IsCancelledStatus(status),
            FailureCode: null,
            FailureMessage: null));
    }

    private async Task<JsonElement> SendApiRequestAsync(
        Dictionary<string, object?> payload,
        CancellationToken ct)
    {
        var data = EncodePayload(payload);
        var signature = Sign(data);

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["data"] = data,
            ["signature"] = signature
        });

        using var response = await _httpClient.PostAsync(_options.ApiUrl, content, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    private string EncodePayload(Dictionary<string, object?> payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    private string DecodePayload(string data)
    {
        var bytes = Convert.FromBase64String(data);
        return Encoding.UTF8.GetString(bytes);
    }

    private string Sign(string data)
    {
        var signString = $"{_options.PrivateKey}{data}{_options.PrivateKey}";
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(signString));
        return Convert.ToBase64String(hash);
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        var a = Encoding.UTF8.GetBytes(left);
        var b = Encoding.UTF8.GetBytes(right);
        return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
    }

    private static Dictionary<string, string> ParseFormEncoded(string payload)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var pair in payload.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = pair.Split('=', 2);
            var key = Uri.UnescapeDataString(parts[0]);
            var value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
            result[key] = value;
        }

        return result;
    }

    private static string? GetString(JsonElement root, string propertyName)
        => root.TryGetProperty(propertyName, out var value) && value.ValueKind != JsonValueKind.Null
            ? value.GetString()
            : null;

    private static bool IsAuthorizedStatus(string? status)
        => status is "hold_wait";

    private static bool IsPaidStatus(string? status)
        => status is "success" or "sandbox";

    private static bool IsFailedStatus(string? status)
        => status is "failure" or "error";

    private static bool IsCancelledStatus(string? status)
        => status is "reversed" or "unsubscribed";
}*/