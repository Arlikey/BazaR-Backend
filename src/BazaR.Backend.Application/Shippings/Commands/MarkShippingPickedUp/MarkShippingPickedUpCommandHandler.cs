using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Domain.Shippings;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Commands.MarkShippingPickedUp;

public sealed class MarkShippingPickedUpCommandHandler
    : IRequestHandler<MarkShippingPickedUpCommand, Result>
{
    private readonly IShippingRepository _shippings;
    private readonly IOrderRepository _orders;
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public MarkShippingPickedUpCommandHandler(
        IShippingRepository shippings,
        IOrderRepository orders,
        ISellerRepository sellers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _shippings = shippings;
        _orders = orders;
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(MarkShippingPickedUpCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller was not found for current user."));

        var shipping = await _shippings.GetByIdAsync(new ShippingId(request.ShippingId), ct);
        if (shipping is null)
            return Result.Failure(new Error("Shipping.NotFound", "Shipping was not found."));

        if (shipping.SellerId != seller.Id)
            return Result.Failure(new Error("Shipping.Forbidden", "You do not own this shipping."));

        var shippingResult = shipping.MarkPickedUp();
        if (shippingResult.IsFailure)
            return shippingResult;

        var order = await _orders.GetByIdAsync(shipping.OrderId, ct);
        if (order is null)
            return Result.Failure(new Error("Order.NotFound", "Order was not found."));

        var orderResult = order.Deliver();
        if (orderResult.IsFailure)
            return orderResult;

        _shippings.Update(shipping);
        _orders.Update(order);

        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}