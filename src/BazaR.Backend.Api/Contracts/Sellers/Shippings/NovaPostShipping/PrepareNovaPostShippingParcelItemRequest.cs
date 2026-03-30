namespace BazaR.Backend.Api.Contracts.Sellers.Shippings.NovaPostShipping
{
    public sealed record PrepareNovaPostShippingParcelItemRequest(
    string Description,
    decimal InsuranceCost,
    int RowNumber,
    decimal Width,
    decimal Length,
    decimal Height,
    decimal ActualWeight,
    decimal? VolumetricWeight);
}
