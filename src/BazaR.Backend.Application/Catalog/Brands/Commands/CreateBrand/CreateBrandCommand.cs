using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Domain.Common;
using MediatR;

public sealed record CreateBrandCommand(
    string Name,
    string? Slug,
    UploadFile? Logo,
    string? Description
) : IRequest<Result<Guid>>;