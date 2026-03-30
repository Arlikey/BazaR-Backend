using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.SetShippingParcels;

public sealed record SetShippingParcelsCommand(
    Guid ShippingId,
    IReadOnlyCollection<SetShippingParcelItem> Parcels
) : IRequest<Result>;

public sealed record SetShippingParcelItem(
    int RowNumber,
    string CargoCategory,
    string Description,
    decimal InsuranceCost,
    decimal Width,
    decimal Length,
    decimal Height,
    decimal ActualWeight,
    decimal VolumetricWeight);