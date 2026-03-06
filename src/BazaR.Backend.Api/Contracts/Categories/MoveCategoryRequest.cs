namespace BazaR.Backend.Api.Contracts.Categories;

public sealed record MoveCategoryRequest(
    Guid? NewParentCategoryId,
    int NewSortOrder = 0
);
