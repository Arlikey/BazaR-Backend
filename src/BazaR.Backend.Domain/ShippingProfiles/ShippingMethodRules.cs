namespace BazaR.Backend.Domain.ShippingProfiles;

public sealed record ShippingMethodRules(
    bool RequiresCity,
    bool RequiresPickupPoint,
    bool RequiresStreetAddress,
    bool SupportsCashOnDelivery)
{
    public static ShippingMethodRules For(ShippingMethodType methodType)
    {
        return methodType switch
        {
            ShippingMethodType.PickupBazar =>
                new ShippingMethodRules(
                    RequiresCity: false,
                    RequiresPickupPoint: false,
                    RequiresStreetAddress: false,
                    SupportsCashOnDelivery: false),

            ShippingMethodType.NovaPoshtaWarehouse =>
                new ShippingMethodRules(
                    RequiresCity: true,
                    RequiresPickupPoint: true,
                    RequiresStreetAddress: false,
                    SupportsCashOnDelivery: true),

            ShippingMethodType.NovaPoshtaLocker =>
                new ShippingMethodRules(
                    RequiresCity: true,
                    RequiresPickupPoint: true,
                    RequiresStreetAddress: false,
                    SupportsCashOnDelivery: true),

            ShippingMethodType.NovaPoshtaCourier =>
                new ShippingMethodRules(
                    RequiresCity: true,
                    RequiresPickupPoint: false,
                    RequiresStreetAddress: true,
                    SupportsCashOnDelivery: true),

            ShippingMethodType.UkrPoshtaBranch =>
                new ShippingMethodRules(
                    RequiresCity: true,
                    RequiresPickupPoint: true,
                    RequiresStreetAddress: false,
                    SupportsCashOnDelivery: false),

            ShippingMethodType.UkrPoshtaCourier =>
                new ShippingMethodRules(
                    RequiresCity: true,
                    RequiresPickupPoint: false,
                    RequiresStreetAddress: true,
                    SupportsCashOnDelivery: false),

            _ =>
                new ShippingMethodRules(
                    RequiresCity: false,
                    RequiresPickupPoint: false,
                    RequiresStreetAddress: false,
                    SupportsCashOnDelivery: false)
        };
    }
}