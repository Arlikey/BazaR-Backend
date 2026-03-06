using BazaR.Backend.Domain.Catalog.Products;

public sealed record ProductSearchFilter(
    string Term,
    ProductStatus Status
);