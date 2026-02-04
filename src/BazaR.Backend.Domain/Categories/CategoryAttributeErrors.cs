using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Categories;

public static class CategoryAttributeErrors
{
    public static readonly Error SortOrderCannotBeNegative =
        new("CategoryAttribute.SortOrderCannotBeNegative", "Sort order cannot be negative.");

    public static readonly Error SectionNameTooLong =
        new("CategoryAttribute.SectionNameTooLong", "Section name is too long.");

    public static readonly Error SectionOrderCannotBeNegative =
        new("CategoryAttribute.SectionOrderCannotBeNegative", "Section order cannot be negative.");

    public static readonly Error DuplicateAttributeInCategory =
        new("CategoryAttribute.DuplicateAttributeInCategory", "This attribute is already assigned to the category.");
}
