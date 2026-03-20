using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.Checkout;

public sealed class CheckoutCartCommandHandler
    : IRequestHandler<CheckoutCartCommand, Result<Guid>>
{
    private readonly ICartRepository _carts;
    private readonly IOrderRepository _orders;
    private readonly IOfferRepository _offers;
    private readonly IProductRepository _products;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public CheckoutCartCommandHandler(
        ICartRepository carts,
        IOrderRepository orders,
        IOfferRepository offers,
        IProductRepository products,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _carts = carts;
        _orders = orders;
        _offers = offers;
        _products = products;
        _uow = uow;
        _current = current;
    }

    public async Task<Result<Guid>> Handle(CheckoutCartCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
        {
            return Result<Guid>.Failure(
                new Error("Auth.Required", "Authentication required."));
        }

        var buyerUserId = new UserId(_current.UserId);

        var cart = await _carts.GetActiveByUserIdAsync(buyerUserId, ct);
        if (cart is null)
        {
            return Result<Guid>.Failure(
                new Error("Cart.NotFound", "Active cart not found."));
        }

        if (cart.Items.Count == 0)
            return Result<Guid>.Failure(CartErrors.EmptyCart);

        var customerRes = OrderCustomer.Create(
            firstName: request.Customer.FirstName,
            lastName: request.Customer.LastName,
            email: request.Customer.Email,
            phone: request.Customer.Phone);

        if (customerRes.IsFailure)
            return Result<Guid>.Failure(customerRes.Error);

        var deliveryRes = OrderDelivery.Create(
            method: request.Delivery.Method,
            city: request.Delivery.City,
            region: request.Delivery.Region,
            warehouse: request.Delivery.Warehouse,
            street: request.Delivery.Street,
            building: request.Delivery.Building,
            apartment: request.Delivery.Apartment,
            postalCode: request.Delivery.PostalCode);

        if (deliveryRes.IsFailure)
            return Result<Guid>.Failure(deliveryRes.Error);

        var offerIds = cart.Items
            .Select(i => i.OfferId)
            .Distinct()
            .ToList();

        var offers = await Task.WhenAll(
            offerIds.Select(id => _offers.GetByIdAsync(id, ct)));

        if (offers.Any(o => o is null))
        {
            return Result<Guid>.Failure(
                new Error("Offer.NotFound", "Some offers were not found."));
        }

        var offerDict = offers
            .Select(o => o!)
            .ToDictionary(o => o.Id, o => o);

        var productIds = offers
            .Select(o => o!.ProductId)
            .Distinct()
            .ToList();

        var products = await Task.WhenAll(
            productIds.Select(id => _products.GetByIdAsync(id, ct)));

        if (products.Any(p => p is null))
        {
            return Result<Guid>.Failure(
                new Error("Product.NotFound", "Some products were not found."));
        }

        var productDict = products
            .Select(p => p!)
            .ToDictionary(p => p.Id, p => p);

        var lines = new List<OrderLine>(cart.Items.Count);

        foreach (var cartItem in cart.Items)
        {
            if (!offerDict.TryGetValue(cartItem.OfferId, out var offer))
            {
                return Result<Guid>.Failure(
                    new Error("Offer.NotFound", $"Offer '{cartItem.OfferId.Value}' not found."));
            }

            if (offer.Status != OfferStatus.Active)
            {
                return Result<Guid>.Failure(
                    new Error("Offer.NotActive", $"Offer '{offer.Id.Value}' is not active."));
            }

            if (cartItem.Quantity < offer.MinOrderQuantity)
            {
                return Result<Guid>.Failure(
                    new Error(
                        "Offer.MinOrderQuantity",
                        $"Minimum order quantity for offer '{offer.Id.Value}' is {offer.MinOrderQuantity}."));
            }

            if (offer.Stock < cartItem.Quantity)
            {
                return Result<Guid>.Failure(
                    new Error(
                        "Offer.NotEnoughStock",
                        $"Not enough stock for offer '{offer.Id.Value}'."));
            }

            if (!productDict.TryGetValue(offer.ProductId, out var product))
            {
                return Result<Guid>.Failure(
                    new Error("Product.NotFound", $"Product '{offer.ProductId.Value}' not found."));
            }

            var priceRes = Money.Create(
                cartItem.PriceSnapshot.Amount,
                cartItem.PriceSnapshot.Currency);

            if (priceRes.IsFailure)
                return Result<Guid>.Failure(priceRes.Error);

            lines.Add(new OrderLine(
                OfferId: offer.Id,
                ProductId: offer.ProductId,
                SellerId: offer.SellerId,
                ProductName: product.Name,
                Sku: offer.SellerSku,
                Quantity: cartItem.Quantity,
                UnitPrice: priceRes.Value!
            ));
        }

        var orderRes = Order.Create(
            buyerUserId: buyerUserId,
            customer: customerRes.Value!,
            delivery: deliveryRes.Value!,
            lines: lines,
            customerComment: request.CustomerComment);

        if (orderRes.IsFailure)
            return Result<Guid>.Failure(orderRes.Error);

        var order = orderRes.Value!;

        foreach (var cartItem in cart.Items)
        {
            var offer = offerDict[cartItem.OfferId];

            var stockRes = offer.DecreaseStock(cartItem.Quantity);
            if (stockRes.IsFailure)
                return Result<Guid>.Failure(stockRes.Error);

            _offers.Update(offer);
        }

        var checkoutRes = cart.MarkCheckedOut();
        if (checkoutRes.IsFailure)
            return Result<Guid>.Failure(checkoutRes.Error);

        _orders.Add(order);
        _carts.Update(cart);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(order.Id.Value);
    }
}