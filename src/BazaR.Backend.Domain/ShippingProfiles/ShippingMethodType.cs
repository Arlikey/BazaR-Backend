namespace BazaR.Backend.Domain.ShippingProfiles;

public enum ShippingMethodType
{
    Unknown = 0,

    PickupBazar = 1,

    NovaPoshtaWarehouse = 2,
    NovaPoshtaLocker = 3,
    NovaPoshtaCourier = 4,

    UkrPoshtaBranch = 5,
    UkrPoshtaCourier = 6,
    BazaRPickup = 7,
    BazaRCourier = 8
}