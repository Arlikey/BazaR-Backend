using BazaR.Backend.Application.Catalog.Offers.DTOs;
using BazaR.Backend.Application.Catalog.Products.DTOs;

namespace BazaR.Backend.Application.Abstractions.ReadModels;



// 1/2) лёгкий список (таблица)
public sealed record ProductListItemDto(
    Guid Id,
    string Name,
    string? Description,
    Guid CategoryId,
    Guid? BrandId,
    string? VendorCode,
    string? Slug,
    string Status,
    string? MainImageUrl);

public sealed record ProductCardDto(
    Guid Id,
    string Name,
    string? Slug,
    string? Description,
    string? MainImageUrl
);

public sealed record ProductCardWithOfferDto(
    Guid Id,
    string Name,
    string? Slug,
    string? Description,
    string? MainImageUrl,
    OfferCardDto? Offer
);

/*public sealed record ProductCardWithOfferDto(
    Guid Id,
    string Name,
    string? Slug,
    string? Description,
    string? MainImageUrl,
    OfferCardDto? Offer,
    bool IsFavorite
);*/

public sealed record ProductDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    Guid CategoryId,
    Guid? BrandId,
    string? VendorCode,
    string? Slug,
    string Status,
    string? MainImageUrl,
    IReadOnlyList<ProductImageDto> Images
);


public sealed record ProductAttributesViewDto(
    Guid ProductId,
    Guid CategoryId,
    IReadOnlyList<ProductAttributeViewItemDto> Attributes);

public sealed record ProductAttributeViewItemDto(
    Guid AttributeId,
    string Name,
    string Code,
    string ValueType,
    string? Unit,

    // правила категории
    bool IsRequired,
    bool IsFilterable,
    int SortOrder,
    string? SectionName,
    int? SectionOrder,

    // текущее значение товара
    string? TextValue,
    decimal? NumberValue,
    bool? BoolValue,
    Guid? OptionId,
    IReadOnlyList<Guid> OptionIds,

    // options (для select/multiselect), иначе пусто
    IReadOnlyList<AttributeOptionDto> Options);
