using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Catalog.Products;

namespace BazaR.Backend.Domain.Sales.Events;

public sealed record OfferCreatedEvent(
    OfferId OfferId,
    ProductId ProductId,
    SellerId SellerId
) : DomainEvent;
