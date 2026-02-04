using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Commands.DeleteAttributeDefinition;

public sealed class DeleteAttributeDefinitionCommandHandler
    : IRequestHandler<DeleteAttributeDefinitionCommand, Result>
{
    private readonly IAttributeDefinitionRepository _attributes;
    private readonly IAttributeUsageChecker _usageChecker;
    private readonly IUnitOfWork _uow;

    public DeleteAttributeDefinitionCommandHandler(
        IAttributeDefinitionRepository attributes,
        IAttributeUsageChecker usageChecker,
        IUnitOfWork uow)
    {
        _attributes = attributes;
        _usageChecker = usageChecker;
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteAttributeDefinitionCommand request, CancellationToken ct)
    {
        // Загружаем агрегат
        var attribute = await _attributes.GetByIdAsync(request.AttributeId, ct);
        if (attribute is null)
            return Result.Failure(AttributeErrors.NotFound);

        // Системный атрибут нельзя удалять
        if (attribute.IsSystem)
            return Result.Failure(AttributeErrors.CannotModifySystemAttribute);

        // Атрибут не должен использоваться
        if (await _usageChecker.IsUsedInCategoriesAsync(attribute.Id, ct))
            return Result.Failure(AttributeErrors.AttributeUsedInCategories);

        if (await _usageChecker.IsUsedInProductsAsync(attribute.Id, ct))
            return Result.Failure(AttributeErrors.AttributeUsedInProducts);

        // Удаляем и сохраняем изменения
        await _attributes.RemoveAsync(attribute, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}
