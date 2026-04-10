using System.ComponentModel.DataAnnotations;

namespace BazaR.Backend.Api.Contracts.Categories;

public sealed record CreateCategoryRequest(
    string Name,
    Guid? ParentCategoryId,
    int SortOrder,
    string? Slug);
