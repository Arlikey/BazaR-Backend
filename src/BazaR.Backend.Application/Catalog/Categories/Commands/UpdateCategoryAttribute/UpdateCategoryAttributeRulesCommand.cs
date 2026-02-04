using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.UpdateCategoryAttribute;

public sealed record UpdateCategoryAttributeRulesCommand(
    Guid CategoryId,
    Guid AttributeId,
    bool IsRequired,
    bool IsFilterable,
    int SortOrder,
    string? SectionName,
    int? SectionOrder
) : IRequest<Result>;
