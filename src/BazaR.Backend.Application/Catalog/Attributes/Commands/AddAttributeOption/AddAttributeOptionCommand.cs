using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Commands.AddAttributeOptions;

public sealed record AddAttributeOptionsCommand(
    Guid AttributeId,
    IReadOnlyList<AddAttributeOptionItem> Options
) : IRequest<Result<IReadOnlyList<Guid>>>;

public sealed record AddAttributeOptionItem(string Value, int SortOrder = 0);
