using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.SetImage;

public sealed record SetCategoryImageCommand(
    CategoryId CategoryId,
    UploadFile Image
) : IRequest<Result>;