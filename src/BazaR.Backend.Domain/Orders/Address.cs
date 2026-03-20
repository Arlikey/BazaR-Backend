/*using System.Collections.Generic;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Orders;

public sealed class Address : ValueObject
{
    public string Country { get; }
    public string City { get; }
    public string Street { get; }
    public string? Apartment { get; }

    private Address(string country, string city, string street, string? apartment)
    {
        Country = country;
        City = city;
        Street = street;
        Apartment = apartment;
    }

    private Address() { }
    public static Result<Address> Create(string country, string city, string street, string? apartment = null)
    {
        if (string.IsNullOrWhiteSpace(country)) return Result<Address>.Failure(OrderErrors.AddressInvalid);
        if (string.IsNullOrWhiteSpace(city)) return Result<Address>.Failure(OrderErrors.AddressInvalid);
        if (string.IsNullOrWhiteSpace(street)) return Result<Address>.Failure(OrderErrors.AddressInvalid);

        var c = country.Trim();
        var ci = city.Trim();
        var st = street.Trim();
        var ap = string.IsNullOrWhiteSpace(apartment) ? null : apartment.Trim();

        return Result<Address>.Success(new Address(c, ci, st, ap));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return City;
        yield return Street;
        yield return Apartment;
    }

    public override string ToString()
        => Apartment is null
            ? $"{Country}, {City}, {Street}"
            : $"{Country}, {City}, {Street}, apt {Apartment}";
}
*/