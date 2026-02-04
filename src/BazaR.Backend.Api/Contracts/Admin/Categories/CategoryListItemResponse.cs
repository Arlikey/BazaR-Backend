namespace BazaR.Backend.Api.Contracts.Admin.Categories;
using System.ComponentModel.DataAnnotations;
public sealed record CategoryListItemResponse(
    Guid Id,
    string Name,
    Guid? ParentCategoryId,
    int SortOrder
);
