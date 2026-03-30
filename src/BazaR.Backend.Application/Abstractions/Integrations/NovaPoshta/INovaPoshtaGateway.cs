
using BazaR.Backend.Application.Shippings.DTOs.NovaPoshta;

namespace BazaR.Backend.Application.Abstractions.Integrations.NovaPoshta;

public interface INovaPoshtaGateway
{
    Task<IReadOnlyCollection<NovaPoshtaCityDto>> SearchCitiesAsync(
        string query,
        CancellationToken ct = default);

    Task<IReadOnlyCollection<NovaPoshtaWarehouseDto>> SearchWarehousesAsync(
        string cityRef,
        string? query,
        CancellationToken ct = default);

    Task<NovaPoshtaCreateShipmentResult> CreateShipmentAsync(
        NovaPoshtaCreateShipmentRequest request,
        CancellationToken ct = default);

    Task<NovaPoshtaTrackShipmentResult> TrackShipmentAsync(
        string trackingNumber,
        CancellationToken ct = default);
}