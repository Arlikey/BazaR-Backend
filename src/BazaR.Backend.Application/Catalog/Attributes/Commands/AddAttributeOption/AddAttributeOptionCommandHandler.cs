using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Commands.AddAttributeOptions;

public sealed class AddAttributeOptionsCommandHandler
    : IRequestHandler<AddAttributeOptionsCommand, Result<IReadOnlyList<Guid>>>
{
    private readonly IAttributeDefinitionRepository _attributes;
    private readonly IUnitOfWork _uow;

    public AddAttributeOptionsCommandHandler(IAttributeDefinitionRepository attributes, IUnitOfWork uow)
    {
        _attributes = attributes;
        _uow = uow;
    }


    /// <summary>
    /// Добавляет список опций к атрибуту (только для типов Select/MultiSelect)
    /// </summary>
    public async Task<Result<IReadOnlyList<Guid>>> Handle(AddAttributeOptionsCommand request, CancellationToken ct)
    {
        var attributeId = new AttributeId(request.AttributeId);

        var attribute = await _attributes.GetByIdAsync(attributeId, ct);
        if (attribute is null)
            return Result<IReadOnlyList<Guid>>.Failure(AttributeErrors.NotFound);

        var created = new List<Guid>();

        foreach (var opt in request.Options)
        {
            var res = attribute.AddOption(opt.Value);
            if (res.IsFailure)
                return Result<IReadOnlyList<Guid>>.Failure(res.Error);

            created.Add(res.Value);
        }

        await _uow.SaveChangesAsync(ct);
        return Result<IReadOnlyList<Guid>>.Success(created);
    }
}
