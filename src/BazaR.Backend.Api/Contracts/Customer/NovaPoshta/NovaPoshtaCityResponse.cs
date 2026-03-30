namespace BazaR.Backend.Api.Contracts.Customer.NovaPoshta;

public sealed record NovaPoshtaCityResponse(
    string Ref,
    string Description,
    string? Area,
    string? SettlementType);