using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Shippings;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.SetShippingParcels;

public sealed class SetShippingParcelsCommandHandler
    : IRequestHandler<SetShippingParcelsCommand, Result>
{
    private readonly IShippingRepository _shippings;
    private readonly IUnitOfWork _uow;

    public SetShippingParcelsCommandHandler(
        IShippingRepository shippings,
        IUnitOfWork uow)
    {
        _shippings = shippings;
        _uow = uow;
    }

    public async Task<Result> Handle(
        SetShippingParcelsCommand request,
        CancellationToken ct)
    {
        var shipping = await _shippings.GetByIdAsync(new ShippingId(request.ShippingId), ct);
        if (shipping is null)
        {
            return Result.Failure(new Error(
                "Shipping.NotFound",
                "Shipping was not found."));
        }

        if (request.Parcels is null || request.Parcels.Count == 0)
            return Result.Failure(ShippingErrors.ParcelsRequired);

        var parcels = new List<ShippingParcel>();

        foreach (var item in request.Parcels)
        {
            var parcelResult = ShippingParcel.Create(
                cargoCategory: item.CargoCategory,
                description: item.Description,
                insuranceCost: item.InsuranceCost,
                rowNumber: item.RowNumber,
                width: item.Width,
                length: item.Length,
                height: item.Height,
                actualWeight: item.ActualWeight,
                volumetricWeight: item.VolumetricWeight);

            if (parcelResult.IsFailure)
                return Result.Failure(parcelResult.Error);

            parcels.Add(parcelResult.Value!);
        }

        var result = shipping.SetParcels(parcels);
        if (result.IsFailure)
            return result;

        _shippings.Update(shipping);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}