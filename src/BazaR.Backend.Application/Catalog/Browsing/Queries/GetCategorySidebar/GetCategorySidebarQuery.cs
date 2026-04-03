using BazaR.Backend.Application.Catalog.Browsing.DTOs;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Browsing.Queries.GetCategorySidebar;

public sealed record GetCategorySidebarQuery(Guid CategoryId)
    : IRequest<CategoryCatalogSidebarDto>;