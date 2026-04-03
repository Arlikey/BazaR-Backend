using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.AttachAttribute;

public sealed record AttachAttributeToCategoryCommand(
    Guid CategoryId,
    Guid AttributeId,
    bool IsRequired,
    bool IsFilterable,
    FilterPresentationType? FilterPresentationType,
    bool IsVisibleInSpecifications,
    bool IsVisibleOnProductCard,
    int SortOrder,
    string? SectionName,
    int? SectionOrder) : IRequest<Result>;