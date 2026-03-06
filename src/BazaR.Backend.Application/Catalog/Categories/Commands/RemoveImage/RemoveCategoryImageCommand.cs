using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Categories.Commands.RemoveImage;

public sealed record RemoveCategoryImageCommand(
    CategoryId CategoryId
) : IRequest<Result>;