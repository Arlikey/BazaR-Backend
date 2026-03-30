using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Domain.Shippings;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.DispatchShipping;

public sealed class DispatchShippingCommandHandler
    : IRequestHandler<DispatchShippingCommand, Result>
{
    private readonly IShippingRepository _shippings;
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;

    public DispatchShippingCommandHandler(
        IShippingRepository shippings,
        IOrderRepository orders,
        IUnitOfWork uow)
    {
        _shippings = shippings;
        _orders = orders;
        _uow = uow;
    }

    public async Task<Result> Handle(
        DispatchShippingCommand request,
        CancellationToken ct)
    {
        var shipping = await _shippings.GetByIdAsync(new ShippingId(request.ShippingId), ct);
        if (shipping is null)
        {
            return Result.Failure(new Error(
                "Shipping.NotFound",
                "Shipping was not found."));
        }

        var shippingResult = shipping.Dispatch(request.TrackingNumber);
        if (shippingResult.IsFailure)
            return shippingResult;

        var order = await _orders.GetByIdAsync(shipping.OrderId, ct);
        if (order is null)
        {
            return Result.Failure(new Error(
                "Order.NotFound",
                "Order was not found."));
        }

        var orderResult = order.Ship();
        if (orderResult.IsFailure)
            return orderResult;

        _shippings.Update(shipping);
        await _orders.Update(order);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}