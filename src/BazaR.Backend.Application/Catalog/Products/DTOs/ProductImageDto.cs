namespace BazaR.Backend.Application.Catalog.Products.DTOs;

public sealed record ProductImageDto(
    Guid Id,
    string Url,
    bool IsMain,
    int SortOrder
);