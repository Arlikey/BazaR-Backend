namespace BazaR.Backend.Api.Contracts.Admin.Categories;

public sealed record MoveCategoryRequest(
    Guid? NewParentCategoryId,
    int NewSortOrder = 0
);
