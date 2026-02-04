using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Products;

public sealed class ProductAttributeValue : Entity<Guid>
{
    public AttributeId AttributeId { get; private set; }

    // Типизированные значения (заполнено только одно в зависимости от AttributeDefinition.ValueType)
    public string? TextValue { get; private set; }
    public decimal? NumberValue { get; private set; }
    public bool? BoolValue { get; private set; }
    public Guid? OptionId { get; private set; }

    private readonly List<Guid> _optionIds = new();
    public IReadOnlyCollection<Guid> OptionIds => _optionIds.AsReadOnly();

    private ProductAttributeValue(Guid id, AttributeId attributeId) : base(id)
    {
        AttributeId = attributeId;
    }

    private ProductAttributeValue() { } 

    public static ProductAttributeValue Create(AttributeId attributeId)
        => new(Guid.NewGuid(), attributeId);

    /// <summary>
    /// Устанавливает значение строго по правилам AttributeDefinition.
    /// Тип НЕ хранится здесь, источник истины — def.ValueType.
    /// </summary>
    public Result SetValue(
        AttributeDefinition def,
        string? text = null,
        decimal? number = null,
        bool? boolean = null,
        Guid? optionId = null,
        IReadOnlyCollection<Guid>? optionIds = null)
    {

        if (!def.Id.Equals(AttributeId))
            return Result.Failure(ProductErrors.AttributeMismatch);

        // 1) Валидируем через AttributeDefinition (там тип + options)
        Result validation = def.ValueType switch
        {
            AttributeValueType.Text => def.ValidateText(text),
            AttributeValueType.Number => def.ValidateNumber(number),
            AttributeValueType.Boolean => def.ValidateBoolean(boolean),
            AttributeValueType.Select => def.ValidateOption(optionId),
            AttributeValueType.MultiSelect => def.ValidateOptions(optionIds),
            _ => Result.Failure(AttributeErrors.UnknownValueType)
        };

        if (validation.IsFailure)
            return validation;

        // 2) Сохраняем только нужное поле
        ClearAllValues();

        switch (def.ValueType)
        {
            case AttributeValueType.Text:
                TextValue = text!.Trim();
                break;

            case AttributeValueType.Number:
                NumberValue = number!.Value;
                break;

            case AttributeValueType.Boolean:
                BoolValue = boolean!.Value;
                break;

            case AttributeValueType.Select:
                OptionId = optionId!.Value;
                break;

            case AttributeValueType.MultiSelect:
                _optionIds.AddRange(optionIds!);
                break;
        }

        return Result.Success();
    }

    private void ClearAllValues()
    {
        TextValue = null;
        NumberValue = null;
        BoolValue = null;
        OptionId = null;
        _optionIds.Clear();
    }
}
