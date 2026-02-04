using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.AttachAttribute;

public sealed record AttachAttributeToCategoryCommand(
    Guid CategoryId,
    Guid AttributeId,
    bool IsRequired = false,
    bool IsFilterable = false,
    int SortOrder = 0,
    string? SectionName = null,
    int? SectionOrder = null
) : IRequest<Result>;
