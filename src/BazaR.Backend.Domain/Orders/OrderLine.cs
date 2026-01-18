using BazaR.Backend.Domain.Catalog;

namespace BazaR.Backend.Domain.Orders;

public readonly record struct OrderLine(ProductId ProductId, int Quantity, Money PriceSnapshot);

