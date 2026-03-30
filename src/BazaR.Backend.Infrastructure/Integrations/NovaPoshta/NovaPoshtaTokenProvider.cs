using System.Text.Json;
using BazaR.Backend.Application.Abstractions.Integrations.NovaPoshta;
using Microsoft.Extensions.Options;

namespace BazaR.Backend.Infrastructure.Integrations.NovaPoshta;

public sealed class NovaPoshtaTokenProvider : INovaPoshtaTokenProvider
{
    private readonly HttpClient _http;
    private readonly NovaPoshtaOptions _options;

    private string? _cachedToken;
    private DateTimeOffset _expiresAtUtc = DateTimeOffset.MinValue;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public NovaPoshtaTokenProvider(
        HttpClient http,
        IOptions<NovaPoshtaOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public async Task<string> GetTokenAsync(CancellationToken ct = default)
    {
        if (IsTokenValid())
            return _cachedToken!;

        await _lock.WaitAsync(ct);
        try
        {
            if (IsTokenValid())
                return _cachedToken!;

            var baseUrl = _options.BaseUrl.TrimEnd('/');
            var url = $"{baseUrl}/clients/authorization?apiKey={Uri.EscapeDataString(_options.ApiKey)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await _http.SendAsync(request, ct);
            var raw = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Nova Post auth failed. HTTP {(int)response.StatusCode}: {raw}");

            NovaPoshtaAuthResponse? payload;
            try
            {
                payload = JsonSerializer.Deserialize<NovaPoshtaAuthResponse>(
                    raw,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Nova Post auth response parse failed: {ex.Message}");
            }

            if (payload is null || string.IsNullOrWhiteSpace(payload.Jwt))
                throw new InvalidOperationException("Nova Post auth response does not contain jwt.");

            _cachedToken = payload.Jwt.Trim();

            // По документации токен живёт 1 час; обновляем заранее.
            _expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(55);

            return _cachedToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool IsTokenValid()
        => !string.IsNullOrWhiteSpace(_cachedToken)
           && DateTimeOffset.UtcNow < _expiresAtUtc;

    private sealed record NovaPoshtaAuthResponse(string Jwt);
}