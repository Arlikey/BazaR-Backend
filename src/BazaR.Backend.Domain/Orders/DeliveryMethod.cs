using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Domain.Orders
{
    public enum DeliveryMethod
    {
        Unknown = 0,

        NovaPoshtaWarehouse = 1, // Новая Почта отделение
        NovaPoshtaLocker = 2,    // Новая Почта почтомат
        NovaPoshtaCourier = 3,   // Новая Почта курьер

        UkrPoshta = 4,           // Укрпочта

        Courier = 5,             // Курьер магазина

        Pickup = 6               // Самовывоз
    }
}
