using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Catalog.Attributes;

namespace BazaR.Backend.Domain.Catalog.Attributes.Events;

public sealed record AttributeCreatedEvent(AttributeId AttributeId)
    : DomainEvent;

public sealed record AttributeUpdatedEvent(AttributeId AttributeId)
    : DomainEvent;

public sealed record AttributeOptionAddedEvent(AttributeId AttributeId, Guid OptionId)
    : DomainEvent;

public sealed record AttributeOptionRemovedEvent(AttributeId AttributeId, Guid OptionId)
    : DomainEvent;
 