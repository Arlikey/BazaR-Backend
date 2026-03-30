using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using BazaR.Backend.Application.Abstractions.Integrations.NovaPoshta;
using BazaR.Backend.Application.Shippings.DTOs.NovaPoshta;
using Microsoft.Extensions.Options;

namespace BazaR.Backend.Infrastructure.Integrations.NovaPoshta;

public sealed class NovaPoshtaGateway : INovaPoshtaGateway
{
    private readonly HttpClient _http;
    private readonly NovaPoshtaOptions _options;

    public NovaPoshtaGateway(
        HttpClient http,
        IOptions<NovaPoshtaOptions> options)
    {
        _http = http;
        _options = options.Value;
        _http.Timeout = TimeSpan.FromSeconds(60);
    }

    public async Task<IReadOnlyCollection<NovaPoshtaCityDto>> SearchCitiesAsync(
        string query,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Array.Empty<NovaPoshtaCityDto>();

        var request = new NovaPoshtaApiRequest(
            ApiKey: _options.ApiKey,
            ModelName: "Address",
            CalledMethod: "getSettlements",
            MethodProperties: new
            {
                FindByString = query.Trim(),
                Limit = 20,
                Page = 1
            });

        var response = await SendAsync(request, ct);
        if (!response.Success || response.Data.Count == 0)
            return Array.Empty<NovaPoshtaCityDto>();

        var result = new List<NovaPoshtaCityDto>(response.Data.Count);

        foreach (var item in response.Data)
        {
            var cityRef = GetString(item, "Ref");
            var description =
                GetString(item, "Description") ??
                GetString(item, "DescriptionRu") ??
                GetString(item, "Present");

            if (string.IsNullOrWhiteSpace(cityRef) || string.IsNullOrWhiteSpace(description))
                continue;

            result.Add(new NovaPoshtaCityDto(
                Ref: cityRef,
                Description: description,
                Area: GetString(item, "AreaDescription"),
                SettlementType: GetString(item, "SettlementTypeDescription")));
        }

        return result;
    }

    public async Task<IReadOnlyCollection<NovaPoshtaWarehouseDto>> SearchWarehousesAsync(
        string cityRef,
        string? query,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cityRef))
            return Array.Empty<NovaPoshtaWarehouseDto>();

        var normalizedQuery = string.IsNullOrWhiteSpace(query)
            ? null
            : query.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(normalizedQuery))
            return Array.Empty<NovaPoshtaWarehouseDto>();

        var request = new NovaPoshtaApiRequest(
            ApiKey: _options.ApiKey,
            ModelName: "Address",
            CalledMethod: "getWarehouses",
            MethodProperties: new
            {
                CityRef = cityRef.Trim(),
                FindByString = query!.Trim(),
                Limit = 50,
                Page = 1
            });

        var response = await SendAsync(request, ct);
        if (!response.Success || response.Data.Count == 0)
            return Array.Empty<NovaPoshtaWarehouseDto>();

        var result = new List<NovaPoshtaWarehouseDto>();

        foreach (var item in response.Data)
        {
            var warehouseRef = GetString(item, "Ref");
            var description = GetString(item, "Description");
            var number = GetString(item, "Number") ?? string.Empty;
            var category = GetString(item, "CategoryOfWarehouse");

            if (string.IsNullOrWhiteSpace(warehouseRef) || string.IsNullOrWhiteSpace(description))
                continue;

            var haystack = $"{description} {number} {category}".ToLowerInvariant();
            if (!haystack.Contains(normalizedQuery))
                continue;

            result.Add(new NovaPoshtaWarehouseDto(
                Ref: warehouseRef,
                Number: number,
                Description: description,
                CityRef: GetString(item, "CityRef"),
                CategoryOfWarehouse: category));
        }

        return result.Take(50).ToList();
    }

    public async Task<NovaPoshtaCreateShipmentResult> CreateShipmentAsync(
        NovaPoshtaCreateShipmentRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.SenderCityRef))
            return FailureCreate("SenderCityRef is required.");

        if (string.IsNullOrWhiteSpace(request.SenderRef))
            return FailureCreate("SenderRef is required.");

        if (string.IsNullOrWhiteSpace(request.SenderAddressRef))
            return FailureCreate("SenderAddressRef is required.");

        if (string.IsNullOrWhiteSpace(request.ContactSenderRef))
            return FailureCreate("ContactSenderRef is required.");

        if (string.IsNullOrWhiteSpace(request.SendersPhone))
            return FailureCreate("SendersPhone is required.");

        if (string.IsNullOrWhiteSpace(request.RecipientCityRef))
            return FailureCreate("RecipientCityRef is required.");

        if (string.IsNullOrWhiteSpace(request.RecipientRef))
            return FailureCreate("RecipientRef is required.");

        if (string.IsNullOrWhiteSpace(request.RecipientAddressRef))
            return FailureCreate("RecipientAddressRef is required.");

        if (string.IsNullOrWhiteSpace(request.ContactRecipientRef))
            return FailureCreate("ContactRecipientRef is required.");

        if (string.IsNullOrWhiteSpace(request.RecipientsPhone))
            return FailureCreate("RecipientsPhone is required.");

        if (request.WeightKg <= 0)
            return FailureCreate("Weight must be greater than zero.");

        if (request.DeclaredValue <= 0)
            return FailureCreate("Declared value must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.Description))
            return FailureCreate("Description is required.");

        var apiRequest = new NovaPoshtaApiRequest(
            ApiKey: _options.ApiKey,
            ModelName: "InternetDocument",
            CalledMethod: "save",
            MethodProperties: new
            {
                PayerType = "Recipient",
                PaymentMethod = request.CashOnDeliveryAmount.HasValue ? "Cash" : "NonCash",
                DateTime = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                CargoType = "Cargo",
                Weight = request.WeightKg.ToString("0.###", CultureInfo.InvariantCulture),
                ServiceType = "WarehouseWarehouse",
                SeatsAmount = "1",
                Description = request.Description.Trim(),
                Cost = request.DeclaredValue.ToString("0.##", CultureInfo.InvariantCulture),

                CitySender = request.SenderCityRef,
                Sender = request.SenderRef,
                SenderAddress = request.SenderAddressRef,
                ContactSender = request.ContactSenderRef,
                SendersPhone = request.SendersPhone,

                CityRecipient = request.RecipientCityRef,
                Recipient = request.RecipientRef,
                RecipientAddress = request.RecipientAddressRef,
                ContactRecipient = request.ContactRecipientRef,
                RecipientsPhone = request.RecipientsPhone,

                BackwardDeliveryData = request.CashOnDeliveryAmount.HasValue
                    ? new[]
                    {
                        new
                        {
                            PayerType = "Recipient",
                            CargoType = "Money",
                            RedeliveryString = request.CashOnDeliveryAmount.Value.ToString(
                                "0.##",
                                CultureInfo.InvariantCulture)
                        }
                    }
                    : null
            });

        var response = await SendAsync(apiRequest, ct);

        if (!response.Success)
        {
            return new NovaPoshtaCreateShipmentResult(
                IsSuccess: false,
                TrackingNumber: null,
                ExternalShipmentId: null,
                TrackingUrl: null,
                RawStatusCode: null,
                RawStatusName: null,
                ErrorCode: "NovaPoshta.Api.Error",
                ErrorMessage: JoinErrors(response.Errors));
        }

        var first = response.Data.FirstOrDefault();
        if (first.ValueKind == JsonValueKind.Undefined)
            return FailureCreate("Nova Poshta returned empty shipment response.");

        var trackingNumber =
            GetString(first, "IntDocNumber") ??
            GetString(first, "Number");

        var externalShipmentId = GetString(first, "Ref");

        return new NovaPoshtaCreateShipmentResult(
            IsSuccess: !string.IsNullOrWhiteSpace(trackingNumber),
            TrackingNumber: trackingNumber,
            ExternalShipmentId: externalShipmentId,
            TrackingUrl: string.IsNullOrWhiteSpace(trackingNumber)
                ? null
                : $"https://tracking.novaposhta.ua/#/uk/document/{trackingNumber}",
            RawStatusCode: "created",
            RawStatusName: "Shipment created",
            ErrorCode: string.IsNullOrWhiteSpace(trackingNumber) ? "NovaPoshta.Shipment.CreateFailed" : null,
            ErrorMessage: string.IsNullOrWhiteSpace(trackingNumber)
                ? "Nova Poshta did not return tracking number."
                : null);
    }

    public async Task<NovaPoshtaTrackShipmentResult> TrackShipmentAsync(
        string trackingNumber,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            return FailureTrack("Tracking number is required.");

        var request = new NovaPoshtaApiRequest(
            ApiKey: _options.ApiKey,
            ModelName: "TrackingDocument",
            CalledMethod: "getStatusDocuments",
            MethodProperties: new
            {
                Documents = new[]
                {
                    new
                    {
                        DocumentNumber = trackingNumber.Trim(),
                        Phone = string.Empty
                    }
                }
            });

        var response = await SendAsync(request, ct);

        if (!response.Success)
        {
            return new NovaPoshtaTrackShipmentResult(
                IsSuccess: false,
                TrackingNumber: trackingNumber.Trim(),
                RawStatusCode: null,
                RawStatusName: null,
                IsCreated: false,
                IsInTransit: false,
                IsArrivedAtPickupPoint: false,
                IsDelivered: false,
                ErrorCode: "NovaPoshta.Api.Error",
                ErrorMessage: JoinErrors(response.Errors));
        }

        var first = response.Data.FirstOrDefault();
        if (first.ValueKind == JsonValueKind.Undefined)
            return FailureTrack("Nova Poshta returned empty tracking response.");

        var statusCode = GetString(first, "StatusCode");
        var statusName = GetString(first, "Status");
        var normalized = NormalizeStatus(statusName);

        var isDelivered =
            normalized.Contains("отрим") ||
            normalized.Contains("видан") ||
            normalized.Contains("delivered");

        var isArrivedAtPickupPoint =
            normalized.Contains("прибув") ||
            normalized.Contains("прибыло") ||
            normalized.Contains("arrived");

        var isCreated =
            normalized.Contains("створ") ||
            normalized.Contains("created");

        var isInTransit =
            !isDelivered &&
            !isArrivedAtPickupPoint &&
            !isCreated &&
            !string.IsNullOrWhiteSpace(statusName);

        return new NovaPoshtaTrackShipmentResult(
            IsSuccess: true,
            TrackingNumber: trackingNumber.Trim(),
            RawStatusCode: statusCode,
            RawStatusName: statusName,
            IsCreated: isCreated,
            IsInTransit: isInTransit,
            IsArrivedAtPickupPoint: isArrivedAtPickupPoint,
            IsDelivered: isDelivered,
            ErrorCode: null,
            ErrorMessage: null);
    }

    private async Task<NovaPoshtaApiResponse> SendAsync(
        NovaPoshtaApiRequest request,
        CancellationToken ct)
    {
        try
        {
            using var response = await _http.PostAsJsonAsync(_options.BaseUrl, request, ct);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new NovaPoshtaApiResponse(
                    Success: false,
                    Errors: new[] { $"HTTP {(int)response.StatusCode}: {raw}" },
                    Data: Array.Empty<JsonElement>());
            }

            var payload = JsonSerializer.Deserialize<NovaPoshtaApiResponse>(
                raw,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return payload ?? new NovaPoshtaApiResponse(
                Success: false,
                Errors: new[] { "Empty Nova Poshta response." },
                Data: Array.Empty<JsonElement>());
        }
        catch (TaskCanceledException ex)
        {
            return new NovaPoshtaApiResponse(
                Success: false,
                Errors: new[] { $"Nova Poshta request canceled or timed out: {ex.Message}" },
                Data: Array.Empty<JsonElement>());
        }
        catch (Exception ex)
        {
            return new NovaPoshtaApiResponse(
                Success: false,
                Errors: new[] { $"Nova Poshta request failed: {ex.Message}" },
                Data: Array.Empty<JsonElement>());
        }
    }

    private static string NormalizeStatus(string? status)
        => (status ?? string.Empty).Trim().ToLowerInvariant();

    private static string JoinErrors(IReadOnlyCollection<string>? errors)
        => errors is null || errors.Count == 0
            ? "Unknown Nova Poshta error."
            : string.Join("; ", errors);

    private static string? GetString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var prop))
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

    private static NovaPoshtaCreateShipmentResult FailureCreate(string message)
        => new(
            IsSuccess: false,
            TrackingNumber: null,
            ExternalShipmentId: null,
            TrackingUrl: null,
            RawStatusCode: null,
            RawStatusName: null,
            ErrorCode: "NovaPoshta.Validation",
            ErrorMessage: message);

    private static NovaPoshtaTrackShipmentResult FailureTrack(string message)
        => new(
            IsSuccess: false,
            TrackingNumber: null,
            RawStatusCode: null,
            RawStatusName: null,
            IsCreated: false,
            IsInTransit: false,
            IsArrivedAtPickupPoint: false,
            IsDelivered: false,
            ErrorCode: "NovaPoshta.Validation",
            ErrorMessage: message);

    private sealed record NovaPoshtaApiRequest(
        string ApiKey,
        string ModelName,
        string CalledMethod,
        object MethodProperties);

    private sealed record NovaPoshtaApiResponse(
        bool Success,
        IReadOnlyCollection<string> Errors,
        IReadOnlyCollection<JsonElement> Data);
}