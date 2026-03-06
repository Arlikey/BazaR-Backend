public class OfferReadModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public decimal? PriceAmount { get; set; }
    public string? PriceCurrency { get; set; }
    public decimal? OldPriceAmount { get; set; }
    public int Stock { get; set; }
    public string? SellerSku { get; set; }
    public int? DeliveryDays { get; set; }
    public int MinOrderQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

