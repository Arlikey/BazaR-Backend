using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.ShippingProfiles;
using BazaR.Backend.Domain.Shippings;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Application.Checkouts.Services;

public sealed class OrderFactory : IOrderFactory
{
    public Result<Order> CreateFromCheckoutLine(
        Checkout checkout,
        CheckoutLine line,
        UserId userId,
        DateTimeOffset nowUtc)
    {
        if (checkout is null)
        {
            return Result<Order>.Failure(
                new Error("OrderFactory.Checkout.Required", "Checkout is required."));
        }

        if (line is null)
        {
            return Result<Order>.Failure(
                new Error("OrderFactory.Line.Required", "Checkout line is required."));
        }

        if (line.Recipient is null)
        {
            return Result<Order>.Failure(
                new Error("OrderFactory.Line.Recipient.Required", "Recipient is required."));
        }

        if (line.Shipping is null)
        {
            return Result<Order>.Failure(
                new Error("OrderFactory.Line.Shipping.Required", "Shipping is required."));
        }

        if (line.Payment is null)
        {
            return Result<Order>.Failure(
                new Error("OrderFactory.Line.Payment.Required", "Payment is required."));
        }

        var customerResult = CreateOrderCustomer(line);
        if (customerResult.IsFailure)
            return Result<Order>.Failure(customerResult.Error);

        var deliveryResult = CreateOrderDelivery(line.Shipping);
        if (deliveryResult.IsFailure)
            return Result<Order>.Failure(deliveryResult.Error);

        var lineResult = CreateOrderLine(line);
        if (lineResult.IsFailure)
            return Result<Order>.Failure(lineResult.Error);

        var orderLines = new List<OrderLine> { lineResult.Value! };

        var orderResult = Order.Create(
            buyerUserId: userId,
            customer: customerResult.Value!,
            delivery: deliveryResult.Value!,
            lines: orderLines,
            customerComment: line.Shipping.Comment);

        if (orderResult.IsFailure)
            return Result<Order>.Failure(orderResult.Error);

        return Result<Order>.Success(orderResult.Value!);
    }

    private static Result<OrderCustomer> CreateOrderCustomer(CheckoutLine line)
    {
        var recipient = line.Recipient!;

        return OrderCustomer.Create(
            firstName: recipient.FirstName,
            lastName: recipient.LastName,
            email: recipient.Email ?? string.Empty,
            phone: recipient.Phone);
    }

    private static Result<OrderDelivery> CreateOrderDelivery(ShippingSelection shipping)
    {
        var method = MapDeliveryMethod(shipping.MethodType);

        return OrderDelivery.Create(
            method: method,
            city: shipping.City,
            region: shipping.Region,
            warehouse: shipping.PickupPointName ?? shipping.PickupPointCode,
            street: shipping.Street,
            building: shipping.House,
            apartment: shipping.Apartment,
            postalCode: shipping.PostalCode);
    }

    private static DeliveryMethod MapDeliveryMethod(ShippingMethodType methodType)
    {
        return methodType switch
        {
            ShippingMethodType.NovaPoshtaWarehouse => DeliveryMethod.NovaPoshta,
            ShippingMethodType.NovaPoshtaLocker => DeliveryMethod.NovaPoshta,
            ShippingMethodType.NovaPoshtaCourier => DeliveryMethod.NovaPoshta,
            ShippingMethodType.BazaRCourier => DeliveryMethod.BazaRCourier,
            ShippingMethodType.BazaRPickup => DeliveryMethod.BazaRPickup,
            _ => DeliveryMethod.Unknown
        };
    }

    private static Result<OrderLine> CreateOrderLine(CheckoutLine line)
    {
        try
        {
            var orderLine = new OrderLine(
                line.OfferId,
                line.ProductId,
                line.SellerId,
                line.ProductTitle,
                line.Sku,
                line.Quantity,
                line.UnitPrice);

            return Result<OrderLine>.Success(orderLine);
        }
        catch (Exception ex)
        {
            return Result<OrderLine>.Failure(
                new Error("OrderFactory.Line.Invalid", ex.Message));
        }
    }
}