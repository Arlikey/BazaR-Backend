namespace BazaR.Backend.Domain.Common;

// 1. Абстрактный класс с generic параметром TId
//    где TId : notnull - тип Id не может быть null
public abstract class Entity<TId>
    where TId : notnull
{
    // 2. Идентификатор сущности
    //    protected set - можно менять только внутри класса-наследника
    public TId Id { get; protected set; }

    // 3. Основной конструктор
    protected Entity(TId id)
    {
        Id = id;
    }

    // 4. Конструктор без параметров ДЛЯ EF Core
    //    EF Core требует конструктор без параметров для создания объектов
    protected Entity() { }

    // 5. Переопределение Equals для сравнения сущностей
    //    Две сущности равны, если они одного типа и имеют одинаковый Id
    public override bool Equals(object? obj)
        => obj is Entity<TId> other &&
           GetType() == other.GetType() &&
           EqualityComparer<TId>.Default.Equals(Id, other.Id);

    // 6. Переопределение GetHashCode
    //    Хэш-код зависит от типа и Id
    public override int GetHashCode()
        => HashCode.Combine(GetType(), Id);
}