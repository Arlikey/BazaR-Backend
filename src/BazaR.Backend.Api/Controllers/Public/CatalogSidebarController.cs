using BazaR.Backend.Api.Contracts.Browsing;
using BazaR.Backend.Application.Catalog.Browsing.DTOs;
using BazaR.Backend.Application.Catalog.Browsing.Queries.BrowseCategoryProducts;
using BazaR.Backend.Application.Catalog.Browsing.Queries.GetCategorySidebar;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Catalog;

[ApiController]
[Route("api/catalog/categories")]
public sealed class CatalogSidebarController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogSidebarController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{categoryId:guid}/sidebar")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSidebar(
        [FromRoute] Guid categoryId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetCategorySidebarQuery(categoryId), ct);
        return Ok(result);
    }

    [HttpPost("{categoryId:guid}/products/filter")]
    [AllowAnonymous]
    public async Task<IActionResult> BrowseProducts(
        [FromRoute] Guid categoryId,
        [FromBody] BrowseCategoryProductsRequest request,
        CancellationToken ct)
    {
        var filters = new List<CatalogSelectedFilterDto>();

        // Select
        filters.AddRange(request.SelectFilters.Select(x =>
            new CatalogSelectFilterDto
            {
                AttributeId = x.AttributeId,
                OptionIds = x.OptionIds
            }));

        // MultiSelect
        filters.AddRange(request.MultiSelectFilters.Select(x =>
            new CatalogMultiSelectFilterDto
            {
                AttributeId = x.AttributeId,
                OptionIds = x.OptionIds
            }));

        // Boolean
        filters.AddRange(request.BooleanFilters.Select(x =>
            new CatalogBooleanFilterDto
            {
                AttributeId = x.AttributeId,
                Value = x.Value
            }));

        // Number range
        filters.AddRange(request.NumberRangeFilters.Select(x =>
            new CatalogNumberRangeFilterDto
            {
                AttributeId = x.AttributeId,
                Min = x.Min,
                Max = x.Max
            }));

        // Text
        filters.AddRange(request.TextFilters.Select(x =>
            new CatalogTextFilterDto
            {
                AttributeId = x.AttributeId,
                Values = x.Values
            }));

       
        var systemFilters = new CatalogSystemFiltersDto
        {
            BrandIds = request.BrandIds ?? new List<Guid>(),
            SellerGroups = request.SellerGroups ?? new List<string>(),
            PriceMin = request.PriceMin,
            PriceMax = request.PriceMax
        };

        var result = await _sender.Send(
            new BrowseCategoryProductsQuery(
                categoryId,
                filters,
                systemFilters,
                request.Page,
                request.PageSize,
                request.SortBy),
            ct);

        return Ok(result);
    }
}