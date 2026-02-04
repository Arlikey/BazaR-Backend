using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Categories.Events;

public sealed record CategoryRenamedEvent(CategoryId CategoryId, string Name) : DomainEvent;
