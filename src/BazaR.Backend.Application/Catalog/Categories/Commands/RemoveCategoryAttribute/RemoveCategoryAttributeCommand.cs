using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.RemoveCategoryAttribute;

public sealed record RemoveCategoryAttributeCommand(
    Guid CategoryId,
    Guid AttributeId
) : IRequest<Result>;
