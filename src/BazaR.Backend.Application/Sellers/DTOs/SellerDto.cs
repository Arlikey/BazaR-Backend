using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Application.Sellers.DTOs;

public sealed record SellerDto(
    Guid Id,
    string Name,
    string Slug,
    SellerType Type,
    SellerStatus Status,
    Guid? OwnerUserId,
    string? LegalName,
    string? TaxNumber,
    string CountryCode,
    string? SupportEmail,
    string? SupportPhone,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastDecisionAt,
    Guid? LastDecisionBy,
    string? LastRejectionReason,
    string? SuspensionReason
);



public sealed record SellerListItemDto(
    Guid Id,
    string Name,
    string Slug,
    SellerType Type,
    SellerStatus Status,
    Guid? OwnerUserId,
    DateTimeOffset CreatedAt
);