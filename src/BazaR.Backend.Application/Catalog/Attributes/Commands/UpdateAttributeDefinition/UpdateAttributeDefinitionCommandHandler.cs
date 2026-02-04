using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Commands.UpdateAttributeDefinition;

public sealed class UpdateAttributeDefinitionCommandHandler
    : IRequestHandler<UpdateAttributeDefinitionCommand, Result>
{
    private readonly IAttributeDefinitionRepository _attributes;
    private readonly IUnitOfWork _uow;

    public UpdateAttributeDefinitionCommandHandler(
        IAttributeDefinitionRepository attributes,
        IUnitOfWork uow)
    {
        _attributes = attributes;
        _uow = uow;
    }

    public async Task<Result> Handle(UpdateAttributeDefinitionCommand request, CancellationToken ct)
    {
        // Формируем идентификатор доменной сущности
        var id = new AttributeId(request.AttributeId);

        // Загружаем агрегат
        var attribute = await _attributes.GetByIdAsync(id, ct);
        if (attribute is null)
            return Result.Failure(AttributeErrors.NotFound);

        // Проверка уникальности кода (только если он передан)
        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var codeExists = await _attributes.CodeExistsAsync(request.Code, excludeId: id, ct);
            if (codeExists)
                return Result.Failure(AttributeErrors.CodeAlreadyExists);
        }

        // Обновление имени
        if (request.Name is not null)
        {
            var r = attribute.Rename(request.Name);
            if (r.IsFailure) return r;
        }

        // Обновление кода
        if (request.Code is not null)
        {
            var r = attribute.ChangeCode(request.Code);
            if (r.IsFailure) return r;
        }

        // Обновление единицы измерения (включая сброс в null)
        if (request.Unit is not null || request.Unit == null)
        {
            var r = attribute.ChangeUnit(request.Unit);
            if (r.IsFailure) return r;
        }

        // Обновление типа значения
        if (request.ValueType is not null)
        {
            var r = attribute.ChangeType(request.ValueType.Value);
            if (r.IsFailure) return r;
        }

        // Сохраняем изменения
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}
