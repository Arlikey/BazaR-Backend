namespace BazaR.Backend.Infrastructure.Payments;

public sealed class LiqPayOptions
{
    public const string SectionName = "LiqPay";

    public string PublicKey { get; set; } = default!;
    public string PrivateKey { get; set; } = default!;
    public string CheckoutUrl { get; set; } = "https://www.liqpay.ua/api/3/checkout";
    public string ApiUrl { get; set; } = "https://www.liqpay.ua/api/request";
}