using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Categories.Events;

public sealed record CategoryParentChangedEvent(CategoryId CategoryId, CategoryId? ParentCategoryId, int SortOrder) : DomainEvent;
