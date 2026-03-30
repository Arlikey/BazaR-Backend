namespace BazaR.Backend.Api.Contracts.Customer.NovaPostSandbox;

public sealed record SandboxDivisionResponse(
    string Id,
    string Name,
    string? CountryCode,
    string? SettlementName,
    string? PostalCode,
    string? Address);