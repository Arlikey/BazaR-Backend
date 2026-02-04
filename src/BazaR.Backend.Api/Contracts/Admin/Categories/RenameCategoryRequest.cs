using System.ComponentModel.DataAnnotations;

namespace BazaR.Backend.Api.Contracts.Admin.Categories;

public sealed record RenameCategoryRequest(
    [Required, MaxLength(200)] string Name
);
