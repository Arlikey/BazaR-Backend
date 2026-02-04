using System.Text.RegularExpressions;
using BazaR.Backend.Domain.Catalog.Attributes.Events;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Attributes;

public sealed class AttributeDefinition : AggregateRoot<AttributeId>
{
    private const int MaxNameLength = 200;
    private const int MaxCodeLength = 100;
    private const int MaxUnitLength = 20;

    private static readonly Regex CodeRegex =
        new("^[a-z0-9_]+$", RegexOptions.Compiled);

    private readonly List<AttributeOption> _options = new();
    public IReadOnlyCollection<AttributeOption> Options => _options.AsReadOnly();

    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public AttributeValueType ValueType { get; private set; }
    public string? Unit { get; private set; } // "GB", "inch", "ml"

    /// <summary>
    /// System attribute: нельзя изменять обычными операциями (rename/type/unit/options).
    /// Создаётся сидингом или админом с особыми правами.
    /// </summary>
    public bool IsSystem { get; private set; }

    private AttributeDefinition(
        AttributeId id,
        string name,
        string code,
        AttributeValueType valueType,
        string? unit,
        bool isSystem)
        : base(id)
    {
        Name = name;
        Code = code;
        ValueType = valueType;
        Unit = unit;
        IsSystem = isSystem;
    }

    private AttributeDefinition() { } 

    // =========================
    // Factory
    // =========================
    public static Result<AttributeDefinition> Create(
        string name,
        string code,
        AttributeValueType valueType,
        string? unit = null,
        bool isSystem = false)
    {
        var basics = ValidateBasics(name, code, unit);
        if (basics.IsFailure)
            return Result<AttributeDefinition>.Failure(basics.Error);

        var attribute = new AttributeDefinition(
            AttributeId.New(),
            name.Trim(),
            NormalizeCode(code),
            valueType,
            NormalizeUnit(unit),
            isSystem);

        attribute.AddDomainEvent(new AttributeCreatedEvent(attribute.Id));
        return Result<AttributeDefinition>.Success(attribute);
    }

    // =========================
    // Mutations (guarded by IsSystem)
    // =========================
    public Result Rename(string name)
    {
        if (IsSystem)
            return Result.Failure(AttributeErrors.CannotModifySystemAttribute);

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(AttributeErrors.NameRequired);

        var trimmed = name.Trim();
        if (trimmed.Length > MaxNameLength)
            return Result.Failure(AttributeErrors.NameTooLong);

        if (Name == trimmed)
            return Result.Success();

        Name = trimmed;
        AddDomainEvent(new AttributeUpdatedEvent(Id));
        return Result.Success();
    }

    public Result ChangeCode(string code)
    {
        if (IsSystem)
            return Result.Failure(AttributeErrors.CannotModifySystemAttribute);

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure(AttributeErrors.CodeRequired);

        var normalized = NormalizeCode(code);
        if (normalized.Length > MaxCodeLength)
            return Result.Failure(AttributeErrors.CodeTooLong);

        if (!CodeRegex.IsMatch(normalized))
            return Result.Failure(AttributeErrors.InvalidCodeFormat);

        if (Code == normalized)
            return Result.Success();

        Code = normalized;
        AddDomainEvent(new AttributeUpdatedEvent(Id));
        return Result.Success();
    }

    public Result ChangeUnit(string? unit)
    {
        if (IsSystem)
            return Result.Failure(AttributeErrors.CannotModifySystemAttribute);

        var normalized = NormalizeUnit(unit);
        if (normalized is not null && normalized.Length > MaxUnitLength)
            return Result.Failure(AttributeErrors.UnitTooLong);

        if (Unit == normalized)
            return Result.Success();

        Unit = normalized;
        AddDomainEvent(new AttributeUpdatedEvent(Id));
        return Result.Success();
    }

    public Result ChangeType(AttributeValueType newType)
    {
        if (IsSystem)
            return Result.Failure(AttributeErrors.CannotModifySystemAttribute);

        if (ValueType == newType)
            return Result.Success();

        // Инвариант: если есть options — тип менять нельзя
        if (_options.Count > 0)
            return Result.Failure(AttributeErrors.CannotChangeTypeWithOptions);

        ValueType = newType;
        AddDomainEvent(new AttributeUpdatedEvent(Id));
        return Result.Success();
    }

    /// <summary>
    /// Делает атрибут системным (например, при сидинге).
    /// Можно вызывать только из доверенного кода (seed/admin-super).
    /// </summary>
    public Result MarkAsSystem()
    {
        if (IsSystem)
            return Result.Success();

        IsSystem = true;
        AddDomainEvent(new AttributeUpdatedEvent(Id));
        return Result.Success();
    }

    // =========================
    // Options (Select / MultiSelect)
    // =========================
    public Result<Guid> AddOption(string value)
    {
        if (IsSystem)
            return Result<Guid>.Failure(AttributeErrors.CannotModifySystemAttribute);

        if (!AllowsOptions(ValueType))
            return Result<Guid>.Failure(AttributeErrors.OptionsNotAllowedForType);

        var created = AttributeOption.Create(value);
        if (created.IsFailure)
            return Result<Guid>.Failure(created.Error);

        var option = created.Value;

        var exists = _options.Any(o =>
            string.Equals(o.Value, option.Value, StringComparison.OrdinalIgnoreCase));

        if (exists)
            return Result<Guid>.Failure(AttributeErrors.OptionAlreadyExists);

        _options.Add(option);
        AddDomainEvent(new AttributeOptionAddedEvent(Id, option.Id));

        return Result<Guid>.Success(option.Id);
    }

    public Result RemoveOption(Guid optionId)
    {
        if (IsSystem)
            return Result.Failure(AttributeErrors.CannotModifySystemAttribute);

        if (!AllowsOptions(ValueType))
            return Result.Failure(AttributeErrors.OptionsNotAllowedForType);

        var option = _options.SingleOrDefault(x => x.Id == optionId);
        if (option is null)
            return Result.Failure(AttributeErrors.OptionNotFound);

        _options.Remove(option);
        AddDomainEvent(new AttributeOptionRemovedEvent(Id, optionId));
        return Result.Success();
    }

    // =========================
    // Typed validation
    // =========================
    public Result ValidateText(string? value)
    {
        if (ValueType != AttributeValueType.Text)
            return Result.Success();

        return string.IsNullOrWhiteSpace(value)
            ? Result.Failure(AttributeErrors.ValueRequired)
            : Result.Success();
    }

    public Result ValidateNumber(decimal? value)
    {
        if (ValueType != AttributeValueType.Number)
            return Result.Success();

        return value is null
            ? Result.Failure(AttributeErrors.ValueRequired)
            : Result.Success();
    }

    public Result ValidateBoolean(bool? value)
    {
        if (ValueType != AttributeValueType.Boolean)
            return Result.Success();

        return value is null
            ? Result.Failure(AttributeErrors.ValueRequired)
            : Result.Success();
    }

    public Result ValidateOption(Guid? optionId)
    {
        if (ValueType != AttributeValueType.Select)
            return Result.Success();

        if (optionId is null)
            return Result.Failure(AttributeErrors.ValueRequired);

        return _options.Any(o => o.Id == optionId.Value)
            ? Result.Success()
            : Result.Failure(AttributeErrors.ValueNotInOptions);
    }

    public Result ValidateOptions(IReadOnlyCollection<Guid>? optionIds)
    {
        if (ValueType != AttributeValueType.MultiSelect)
            return Result.Success();

        if (optionIds is null || optionIds.Count == 0)
            return Result.Failure(AttributeErrors.InvalidMultiSelect);

        var distinct = optionIds.Distinct().ToList();
        if (distinct.Count != optionIds.Count)
            return Result.Failure(AttributeErrors.ValueNotInOptions);

        foreach (var id in distinct)
        {
            if (!_options.Any(o => o.Id == id))
                return Result.Failure(AttributeErrors.ValueNotInOptions);
        }

        return Result.Success();
    }

    // =========================
    // Helpers
    // =========================
    private static Result ValidateBasics(string name, string code, string? unit)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(AttributeErrors.NameRequired);

        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength)
            return Result.Failure(AttributeErrors.NameTooLong);

        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure(AttributeErrors.CodeRequired);

        var normalizedCode = NormalizeCode(code);
        if (normalizedCode.Length > MaxCodeLength)
            return Result.Failure(AttributeErrors.CodeTooLong);

        if (!CodeRegex.IsMatch(normalizedCode))
            return Result.Failure(AttributeErrors.InvalidCodeFormat);

        var normalizedUnit = NormalizeUnit(unit);
        if (normalizedUnit is not null && normalizedUnit.Length > MaxUnitLength)
            return Result.Failure(AttributeErrors.UnitTooLong);

        return Result.Success();
    }

    private static string NormalizeCode(string code)
        => code.Trim().ToLowerInvariant();

    private static string? NormalizeUnit(string? unit)
        => string.IsNullOrWhiteSpace(unit) ? null : unit.Trim();

    private static bool AllowsOptions(AttributeValueType type)
        => type is AttributeValueType.Select or AttributeValueType.MultiSelect;
}
