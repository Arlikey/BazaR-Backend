namespace BazaR.Backend.Application.Shippings.DTOs;

public sealed record ShippingParcelDto(
    int RowNumber,
    string CargoCategory,
    string Description,
    decimal InsuranceCost,
    decimal Width,
    decimal Length,
    decimal Height,
    decimal ActualWeight,
    decimal VolumetricWeight);