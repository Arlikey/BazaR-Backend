using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Shippings;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.SetShippingSender;

public sealed class SetShippingSenderCommandHandler
    : IRequestHandler<SetShippingSenderCommand, Result>
{
    private readonly IShippingRepository _shippings;
    private readonly IUnitOfWork _uow;

    public SetShippingSenderCommandHandler(
        IShippingRepository shippings,
        IUnitOfWork uow)
    {
        _shippings = shippings;
        _uow = uow;
    }

    public async Task<Result> Handle(
        SetShippingSenderCommand request,
        CancellationToken ct)
    {
        var shipping = await _shippings.GetByIdAsync(new ShippingId(request.ShippingId), ct);
        if (shipping is null)
        {
            return Result.Failure(new Error(
                "Shipping.NotFound",
                "Shipping was not found."));
        }

        var senderResult = ShippingSender.Create(
            request.Name,
            request.Phone,
            request.CountryCode,
            request.PickupPointCode,
            request.PickupPointName);

        if (senderResult.IsFailure)
            return Result.Failure(senderResult.Error);

        var result = shipping.SetSender(senderResult.Value!);
        if (result.IsFailure)
            return result;

        _shippings.Update(shipping);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}