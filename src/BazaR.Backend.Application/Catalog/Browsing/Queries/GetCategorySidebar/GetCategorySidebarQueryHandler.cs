using BazaR.Backend.Application.Catalog.Browsing.Abstractions;
using BazaR.Backend.Application.Catalog.Browsing.DTOs;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Browsing.Queries.GetCategorySidebar;

public sealed class GetCategorySidebarQueryHandler
    : IRequestHandler<GetCategorySidebarQuery, CategoryCatalogSidebarDto>
{
    private readonly ICatalogBrowseReadRepository _catalogBrowse;

    public GetCategorySidebarQueryHandler(ICatalogBrowseReadRepository catalogBrowse)
    {
        _catalogBrowse = catalogBrowse;
    }

    public Task<CategoryCatalogSidebarDto> Handle(
        GetCategorySidebarQuery request,
        CancellationToken ct)
    {
        return _catalogBrowse.GetCategorySidebarAsync(request.CategoryId, ct);
    }
}