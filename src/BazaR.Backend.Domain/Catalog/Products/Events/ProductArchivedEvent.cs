using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Products.Events;

public sealed record ProductArchivedEvent(ProductId ProductId) : DomainEvent;
