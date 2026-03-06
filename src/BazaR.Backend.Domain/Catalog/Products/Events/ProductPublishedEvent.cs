using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;



public sealed record ProductPublishedEvent(ProductId ProductId) : DomainEvent;
