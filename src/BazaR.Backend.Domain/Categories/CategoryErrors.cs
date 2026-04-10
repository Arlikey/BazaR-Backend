using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;

public static class CategoryErrors
{
    public static readonly Error NameRequired =
        new("Category.NameRequired", "Category name is required.");

    public static readonly Error NameTooLong =
        new("Category.NameTooLong", "Category name is too long.");

    public static readonly Error ParentCategoryInvalid =
        new("Category.ParentCategoryInvalid", "Parent category is invalid.");

    public static readonly Error CannotSetSelfAsParent =
        new("Category.CannotSetSelfAsParent", "Category cannot be parent of itself.");

    public static readonly Error SortOrderCannotBeNegative =
        new("Category.SortOrderCannotBeNegative", "Sort order cannot be negative.");
    public static readonly Error ParentCategoryRequired =
    new("Category.ParentCategoryRequired", "Parent category is required for subcategory.");

    public static readonly Error NotFound =
       new("Category.NotFound", "Category was not found.");

  

    public static readonly Error ParentCategoryNotFound = new(
        "Category.ParentCategoryNotFound",
        "Parent category not found");

    public static readonly Error NameAlreadyExists = new(
        "Category.NameAlreadyExists",
        "Category with this name already exists in the same parent category");

    public static readonly Error CategoryNotFound = new(
        "Category.CategoryNotFound",
        "Category not found");

    public static readonly Error CannotDeleteCategoryWithChildren = new(
        "Category.CannotDeleteCategoryWithChildren",
        "Cannot delete category that has child categories");

    public static readonly Error CannotDeleteCategoryWithProducts = new(
        "Category.CannotDeleteCategoryWithProducts",
        "Cannot delete category that contains products");

    public static readonly Error SlugRequired =
       new("Category.SlugRequired", "Slug is required");

    public static readonly Error SlugTooLong =
        new("Category.SlugTooLong", "Slug is too long");

    public static readonly Error InvalidSlugFormat =
        new("Category.InvalidSlugFormat", "Slug must contain only a-z, 0-9 and '-'");

    public static readonly Error SlugAlreadyExists =
       new("Category.SlugAlreadyExists", "A category with this slug already exists.");

    public static readonly Error CannotSetDescendantAsParent =
        new("Category.CannotSetDescendantAsParent", "A category cannot be moved under its own descendant.");

}

