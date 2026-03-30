namespace BazaR.Backend.Api.Contracts.Shippings;

public sealed record SetShippingParcelsRequest(
    IReadOnlyCollection<SetShippingParcelRequestItem> Parcels);

public sealed record SetShippingParcelRequestItem(
    int RowNumber,
    string CargoCategory,
    string Description,
    decimal InsuranceCost,
    decimal Width,
    decimal Length,
    decimal Height,
    decimal ActualWeight,
    decimal VolumetricWeight);