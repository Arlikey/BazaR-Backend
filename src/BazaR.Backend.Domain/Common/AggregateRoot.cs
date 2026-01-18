using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Domain.Common;

// 1. AggregateRoot наследует от Entity<TId>
//    ВСЕ агрегаты являются сущностями, но не все сущности - агрегатами
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    // 2. Список доменных событий (Domain Events)
    //    private - доступ только внутри класса
    //    readonly - ссылка не меняется после создания
    private readonly List<IDomainEvent> _domainEvents = new();

    // 3. Публичное свойство только для чтения
    //    IReadOnlyCollection - нельзя менять извне
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // 4. Конструктор - передает Id в базовый класс Entity
    protected AggregateRoot(TId id) : base(id) { }

    // 5. Конструктор для EF Core
    protected AggregateRoot() { }

    // 6. Метод добавления события (protected - доступ в наследниках)
    protected void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    // 7. Метод очистки событий (публичный - могут вызывать обработчики)
    public void ClearDomainEvents()
        => _domainEvents.Clear();
}