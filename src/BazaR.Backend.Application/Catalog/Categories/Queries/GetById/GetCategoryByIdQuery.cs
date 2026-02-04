using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.GetById;

public sealed record GetCategoryByIdQuery(CategoryId Id)
    : IRequest<Result<CategoryDetailsDto>>;
