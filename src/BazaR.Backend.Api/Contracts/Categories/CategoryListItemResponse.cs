namespace BazaR.Backend.Api.Contracts.Categories;

public sealed record CategoryListItemResponse(
    Guid Id,
    string Name,
    string? Slug,
    Guid? ParentCategoryId,
    int SortOrder,
    string? ImageUrl);