using System.ComponentModel.DataAnnotations;

namespace BazaR.Backend.Api.Contracts.Admin.Categories;

public sealed record CreateCategoryRequest(
    [Required, MaxLength(200)] string Name,
    Guid? ParentCategoryId,
    int SortOrder = 0
);
