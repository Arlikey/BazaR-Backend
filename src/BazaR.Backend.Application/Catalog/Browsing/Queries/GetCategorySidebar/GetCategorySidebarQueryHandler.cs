using BazaR.Backend.Application.Catalog.Browsing.DTOs;
using BazaR.Backend.Application.Catalog.Browsing.Queries.GetCategorySidebar;
using BazaR.Backend.Application.Catalog.Browsing.Sidebar.Abstractions;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Browsing.Queries.GetCategorySidebar;

public sealed class GetCategorySidebarQueryHandler
    : IRequestHandler<GetCategorySidebarQuery, CategoryCatalogSidebarDto>
{
    private readonly ICatalogSidebarReadRepository _catalogSidebar;

    public GetCategorySidebarQueryHandler(ICatalogSidebarReadRepository catalogSidebar)
    {
        _catalogSidebar = catalogSidebar;
    }

    public Task<CategoryCatalogSidebarDto> Handle(
        GetCategorySidebarQuery request,
        CancellationToken ct)
    {
        return _catalogSidebar.GetCategorySidebarAsync(request.CategoryId, ct);
    }
}