namespace BazaR.Backend.Domain.Checkouts;

public sealed record Address(
    string Country,
    string Region,
    string City,
    string Street,
    string House,
    string? Apartment,
    string? PostalCode)
{
    public static Address Create(
        string country,
        string region,
        string city,
        string street,
        string house,
        string? apartment,
        string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new InvalidOperationException("Country is required.");

        if (string.IsNullOrWhiteSpace(city))
            throw new InvalidOperationException("City is required.");

        return new Address(
            country.Trim(),
            (region ?? string.Empty).Trim(),
            city.Trim(),
            (street ?? string.Empty).Trim(),
            (house ?? string.Empty).Trim(),
            string.IsNullOrWhiteSpace(apartment) ? null : apartment.Trim(),
            string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim());
    }
}