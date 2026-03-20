using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Orders;

public sealed class OrderDelivery : ValueObject
{
    public DeliveryMethod Method { get; private set; }
    public string City { get; private set; } = default!;
    public string? Region { get; private set; }
    public string? Warehouse { get; private set; }
    public string? Street { get; private set; }
    public string? Building { get; private set; }
    public string? Apartment { get; private set; }
    public string? PostalCode { get; private set; }

    private OrderDelivery() { }

    private OrderDelivery(
        DeliveryMethod method,
        string city,
        string? region,
        string? warehouse,
        string? street,
        string? building,
        string? apartment,
        string? postalCode)
    {
        Method = method;
        City = city.Trim();
        Region = Normalize(region);
        Warehouse = Normalize(warehouse);
        Street = Normalize(street);
        Building = Normalize(building);
        Apartment = Normalize(apartment);
        PostalCode = Normalize(postalCode);
    }

    public static Result<OrderDelivery> Create(
        DeliveryMethod method,
        string city,
        string? region,
        string? warehouse,
        string? street,
        string? building,
        string? apartment,
        string? postalCode)
    {
        if (method == DeliveryMethod.Unknown)
            return Result<OrderDelivery>.Failure(
                new Error("OrderDelivery.MethodRequired", "Delivery method is required."));

        if (string.IsNullOrWhiteSpace(city))
            return Result<OrderDelivery>.Failure(
                new Error("OrderDelivery.CityRequired", "City is required."));

        switch (method)
        {
            case DeliveryMethod.NovaPoshtaWarehouse:
            case DeliveryMethod.NovaPoshtaLocker:
                if (string.IsNullOrWhiteSpace(warehouse))
                {
                    return Result<OrderDelivery>.Failure(
                        new Error("OrderDelivery.WarehouseRequired", "Warehouse is required."));
                }
                break;

            case DeliveryMethod.NovaPoshtaCourier:
            case DeliveryMethod.Courier:
                if (string.IsNullOrWhiteSpace(street))
                {
                    return Result<OrderDelivery>.Failure(
                        new Error("OrderDelivery.StreetRequired", "Street is required."));
                }

                if (string.IsNullOrWhiteSpace(building))
                {
                    return Result<OrderDelivery>.Failure(
                        new Error("OrderDelivery.BuildingRequired", "Building is required."));
                }
                break;

            case DeliveryMethod.Pickup:
                break;

            case DeliveryMethod.UkrPoshta:
                // тут можешь потом усилить правила
                break;
        }

        return Result<OrderDelivery>.Success(new OrderDelivery(
            method,
            city,
            region,
            warehouse,
            street,
            building,
            apartment,
            postalCode));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Method;
        yield return City;
        yield return Region;
        yield return Warehouse;
        yield return Street;
        yield return Building;
        yield return Apartment;
        yield return PostalCode;
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}