using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Commands.AddImages;

public sealed class AddProductImagesCommandHandler
    : IRequestHandler<AddProductImagesCommand, Result<IReadOnlyList<Guid>>>
{
    private const int MaxFileSizeBytes = 8 * 1024 * 1024; 
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private readonly IProductRepository _products;
    private readonly ISellerRepository _sellers;
    private readonly ICurrentUser _current;
    private readonly IUnitOfWork _uow;
    private readonly IProductImageStorage _storage;

    public AddProductImagesCommandHandler(
        IProductRepository products,
        ISellerRepository sellers,
        ICurrentUser current,
        IUnitOfWork uow,
        IProductImageStorage storage)
    {
        _products = products;
        _sellers = sellers;
        _current = current;
        _uow = uow;
        _storage = storage;
    }

    public async Task<Result<IReadOnlyList<Guid>>> Handle(AddProductImagesCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<IReadOnlyList<Guid>>.Failure(new Error("Auth.Required", "Authentication required."));

        if (request.Images is null || request.Images.Count == 0)
            return Result<IReadOnlyList<Guid>>.Failure(new Error("Product.Images.Empty", "No images provided."));

        // текущий seller
        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result<IReadOnlyList<Guid>>.Failure(new Error("Seller.NotFound", "Seller not found."));
        if (seller.Status != SellerStatus.Active)
            return Result<IReadOnlyList<Guid>>.Failure(new Error("Seller.NotActive", "Seller must be active."));

        // грузим продукт агрегат
        var product = await _products.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            return Result<IReadOnlyList<Guid>>.Failure(new Error("Product.NotFound", "Product not found."));

        // владелец
        if (product.OwnerSellerId != seller.Id)
            return Result<IReadOnlyList<Guid>>.Failure(new Error("Product.Forbidden", "You are not owner of this product."));

        var uploadedKeys = new List<string>();
        var addedImageIds = new List<Guid>();

        try
        {
            for (var i = 0; i < request.Images.Count; i++)
            {
                var file = request.Images[i];

                if (file.SizeBytes <= 0)
                    return Result<IReadOnlyList<Guid>>.Failure(new Error("Product.Image.Empty", "Image is empty."));

                if (file.SizeBytes > MaxFileSizeBytes)
                    return Result<IReadOnlyList<Guid>>.Failure(new Error("Product.Image.TooLarge", $"Max size is {MaxFileSizeBytes} bytes."));

                if (string.IsNullOrWhiteSpace(file.ContentType) || !AllowedContentTypes.Contains(file.ContentType))
                    return Result<IReadOnlyList<Guid>>.Failure(new Error("Product.Image.InvalidType", "Allowed: jpeg, png, webp."));

                // сохраняем файл
                var stored = await _storage.SaveAsync(
                    productId: product.Id.Value,
                    content: file.Content,
                    originalFileName: file.FileName,
                    contentType: file.ContentType,
                    ct: ct);

                uploadedKeys.Add(stored.StorageKey);

                var makeMain = (i == 0 && request.MakeFirstImageMain);

                var addRes = product.AddImage(
                    url: stored.Url,
                    storageKey: stored.StorageKey,
                    contentType: stored.ContentType,
                    sizeBytes: stored.SizeBytes,
                    makeMain: makeMain);

                if (addRes.IsFailure)
                    return Result<IReadOnlyList<Guid>>.Failure(addRes.Error);

                addedImageIds.Add(addRes.Value);
            }

            _products.Update(product);
            await _uow.SaveChangesAsync(ct);

            return Result<IReadOnlyList<Guid>>.Success(addedImageIds);
        }
        catch
        {
            
            foreach (var key in uploadedKeys)
            {
                try { await _storage.DeleteAsync(key, ct); } catch { }
            }
            throw;
        }
    }
}