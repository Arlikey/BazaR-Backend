namespace BazaR.Backend.Infrastructure.Integrations.NovaPoshta;

public sealed class NovaPoshtaOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.novaposhta.ua/v2.0/json/";
}