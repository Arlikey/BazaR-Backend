using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Domain.Orders;

public sealed record OrderLine(
    OfferId OfferId,
    ProductId ProductId,
    SellerId SellerId,
    string ProductName,
    string? Sku,
    int Quantity,
    Money UnitPrice
);