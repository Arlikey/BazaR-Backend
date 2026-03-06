using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Commands.AddImages;

public sealed record AddProductImagesCommand(
    ProductId ProductId,
    IReadOnlyList<UploadFile> Images,
    bool MakeFirstImageMain = true
) : IRequest<Result<IReadOnlyList<Guid>>>;