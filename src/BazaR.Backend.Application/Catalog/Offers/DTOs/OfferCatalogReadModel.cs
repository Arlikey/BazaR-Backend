using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Catalog.Offers.DTOs
{
    public sealed record OfferCatalogReadModel(
     Guid OfferId,
     Guid ProductId,
     Guid SellerId,

     decimal? PriceAmount,
     string? PriceCurrency,
     decimal? OldPriceAmount,

     int Stock,
     string? SellerSku,
     int? DeliveryDays,
     int MinOrderQuantity,

     string Status
     );


    public sealed record OfferDetailsDto(
    Guid OfferId,
    Guid ProductId,
    Guid SellerId,

    decimal? PriceAmount,
    string? PriceCurrency,
    decimal? OldPriceAmount,

    int Stock,
    string? SellerSku,
    int? DeliveryDays,
    int MinOrderQuantity,

    string Status
);


    public sealed record ProductWithOfferDto(
    Guid Id,
    string Name,
    Guid CategoryId,
    Guid? BrandId,
    string? VendorCode,
    string? Slug,
    string Status,
    OfferCatalogReadModel? Offer);

    /*public sealed record OfferCardDto(
    Guid ProductId,
    decimal? PriceAmount,
    string? PriceCurrency,
    decimal? OldPriceAmount,
    bool InStock
    );*/

    public sealed record OfferCardReadDto(
     Guid ProductId,
     Guid OfferId,
     decimal? PriceAmount,
     string? PriceCurrency,
     decimal? OldPriceAmount,
     int StockQuantity,
     bool IsFavorite);


    public sealed record OfferCardDto(
    Guid Id,
    decimal? PriceAmount,
    string? PriceCurrency,
    decimal? OldPriceAmount,
    int StockQuantity,
    bool IsFavorite
);





}
