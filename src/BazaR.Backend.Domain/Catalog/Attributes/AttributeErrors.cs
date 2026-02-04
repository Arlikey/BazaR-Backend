using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Attributes;

public static class AttributeErrors
{
    public static readonly Error NameRequired =
        new("Attribute.NameRequired", "Attribute name is required.");

    public static readonly Error NameTooLong =
        new("Attribute.NameTooLong", "Attribute name is too long.");

    public static readonly Error CodeRequired =
        new("Attribute.CodeRequired", "Attribute code is required.");

    public static readonly Error CodeTooLong =
        new("Attribute.CodeTooLong", "Attribute code is too long.");

    public static readonly Error InvalidCodeFormat =
        new("Attribute.InvalidCodeFormat", "Attribute code has invalid format. Allowed: a-z, 0-9, _");

    public static readonly Error UnitTooLong =
        new("Attribute.UnitTooLong", "Unit is too long.");

    public static readonly Error CannotModifySystemAttribute =
        new("Attribute.CannotModifySystemAttribute", "System attribute cannot be modified.");

    public static readonly Error CannotChangeTypeWithOptions =
        new("Attribute.CannotChangeTypeWithOptions", "Cannot change attribute type while it has options. Remove options first.");

    public static readonly Error OptionsNotAllowedForType =
        new("Attribute.OptionsNotAllowedForType", "Options are allowed only for Select and MultiSelect types.");

    public static readonly Error OptionValueRequired =
        new("Attribute.OptionValueRequired", "Option value is required.");

    public static readonly Error OptionValueTooLong =
        new("Attribute.OptionValueTooLong", "Option value is too long.");

    public static readonly Error SortOrderCannotBeNegative =
        new("Attribute.SortOrderCannotBeNegative", "Sort order cannot be negative.");

    public static readonly Error OptionAlreadyExists =
        new("Attribute.OptionAlreadyExists", "Option with this value already exists.");

    public static readonly Error OptionNotFound =
        new("Attribute.OptionNotFound", "Option was not found.");

    public static readonly Error ValueRequired =
        new("Attribute.ValueRequired", "Value is required.");

    public static readonly Error ValueNotInOptions =
        new("Attribute.ValueNotInOptions", "Value is not in allowed options.");

    public static readonly Error InvalidMultiSelect =
        new("Attribute.InvalidMultiSelect", "MultiSelect requires at least one option.");

  
    public static readonly Error UnknownValueType =
        new("Attribute.UnknownValueType", "Unknown attribute value type.");

    public static readonly Error NotFound =
    new("Attribute.NotFound", "Attribute definition was not found.");


    public static readonly Error CodeAlreadyExists =
        new("Attribute.CodeAlreadyExists", "Attribute code already exists.");


    public static readonly Error AttributeUsedInCategories =
        new("Attribute.UsedInCategories", "Attribute is used in categories");

    public static readonly Error AttributeUsedInProducts =
        new("Attribute.UsedInProducts", "Attribute is used in products");

}
