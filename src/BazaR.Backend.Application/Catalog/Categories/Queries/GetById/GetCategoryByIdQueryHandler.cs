using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.GetById;

public sealed class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDetailsDto>>
{
    private readonly ICategoryReadRepository _read;

    public GetCategoryByIdQueryHandler(ICategoryReadRepository read) => _read = read;

    public async Task<Result<CategoryDetailsDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var dto = await _read.GetByIdAsync(request.Id, ct);
        if (dto is null)
            return Result<CategoryDetailsDto>.Failure(CategoryErrors.NotFound);

        return Result<CategoryDetailsDto>.Success(dto);
    }
}
