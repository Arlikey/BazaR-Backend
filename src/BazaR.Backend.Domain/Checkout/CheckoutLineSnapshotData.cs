using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;

using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Domain.Checkouts;

public sealed record CheckoutLineSnapshotData(
    OfferId OfferId,
    ProductId ProductId,
    SellerId SellerId,
    string ProductTitle,
    string Sku,
    int Quantity,
    Money UnitPrice);