using BazaR.Backend.Domain.Sellers;


public sealed record SellerListItem(
    Guid Id,
    string Name,
    string Slug,
    SellerType Type,
    SellerStatus Status,
    Guid? OwnerUserId,
    string? TaxNumber,
    string CountryCode,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? SubmittedAt
);
