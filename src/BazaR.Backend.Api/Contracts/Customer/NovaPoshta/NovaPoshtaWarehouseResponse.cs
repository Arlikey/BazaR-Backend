namespace BazaR.Backend.Api.Contracts.Customer.NovaPoshta;

public sealed record NovaPoshtaWarehouseResponse(
    string Ref,
    string Number,
    string Description,
    string? CityRef,
    string? CategoryOfWarehouse);