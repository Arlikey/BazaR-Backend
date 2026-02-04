using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Orders;

public readonly record struct OrderLine(ProductId ProductId, int Quantity, Money PriceSnapshot);

