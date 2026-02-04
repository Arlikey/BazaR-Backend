using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Commands.CreateAttributeDefinition;

public sealed class CreateAttributeDefinitionCommandHandler
    : IRequestHandler<CreateAttributeDefinitionCommand, Result<Guid>>
{
    // Репозиторий для работы с определениями атрибутов (проверки, добавление)
    private readonly IAttributeDefinitionRepository _attributes;

    // Unit of Work для атомарного сохранения изменений
    private readonly IUnitOfWork _uow;

    public CreateAttributeDefinitionCommandHandler(
        IAttributeDefinitionRepository attributes,
        IUnitOfWork uow)
    {
        _attributes = attributes;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateAttributeDefinitionCommand request, CancellationToken ct)
    {
        // Проверяем уникальность Code
        // В домене это сделать нельзя, т.к. требуется доступ к БД
        var codeExists = await _attributes.CodeExistsAsync(
            request.Code,
            excludeId: null,
            ct
        );

        // Если такой Code уже существует — возвращаем доменную ошибку
        if (codeExists)
            return Result<Guid>.Failure(AttributeErrors.CodeAlreadyExists);

        // Создаём доменную сущность через фабричный метод
        // Вся валидация инвариантов происходит внутри домена
        var created = AttributeDefinition.Create(
            request.Name,
            request.Code,
            request.ValueType,
            request.Unit,
            request.IsSystem
        );

        // Если доменная валидация не прошла — пробрасываем ошибку выше
        if (created.IsFailure)
            return Result<Guid>.Failure(created.Error);

        // Добавляем новую сущность в репозиторий
        await _attributes.AddAsync(created.Value, ct);

        // Фиксируем изменения в рамках Unit of Work
        await _uow.SaveChangesAsync(ct);

        // Возвращаем Id созданного атрибута
        return Result<Guid>.Success(created.Value.Id.Value);
    }
}
