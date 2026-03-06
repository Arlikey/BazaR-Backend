using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Categories.Commands.RemoveImage;

public sealed class RemoveCategoryImageCommandHandler : IRequestHandler<RemoveCategoryImageCommand, Result>
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorage _files;

    public RemoveCategoryImageCommandHandler(
        ICategoryRepository categories,
        IUnitOfWork uow,
        IFileStorage files)
    {
        _categories = categories;
        _uow = uow;
        _files = files;
    }

    public async Task<Result> Handle(RemoveCategoryImageCommand request, CancellationToken ct)
    {
        var category = await _categories.GetByIdAsync(request.CategoryId, ct);
        if (category is null)
            return Result.Failure(new Error("Category.NotFound", "Category not found."));

        var oldKey = category.Image?.StorageKey;
        if (string.IsNullOrWhiteSpace(oldKey))
            return Result.Success(); // нечего удалять

        // 1) домен
        var res = category.RemoveImage();
        if (res.IsFailure)
            return Result.Failure(res.Error);

        // 2) коммит
        await _uow.SaveChangesAsync(ct);

        // 3) удаляем файл после коммита (чтобы не потерять ссылку при падении БД)
        try { await _files.DeleteAsync(oldKey, ct); } catch { }

        return Result.Success();
    }
}