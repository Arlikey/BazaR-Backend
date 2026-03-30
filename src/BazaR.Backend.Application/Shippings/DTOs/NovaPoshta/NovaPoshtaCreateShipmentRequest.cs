using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Shippings.DTOs.NovaPoshta
{
    

    public sealed record NovaPoshtaCreateShipmentRequest(
        string SenderCityRef,
        string SenderRef,
        string SenderAddressRef,
        string ContactSenderRef,
        string SendersPhone,

        string RecipientCityRef,
        string RecipientRef,
        string RecipientAddressRef,
        string ContactRecipientRef,
        string RecipientsPhone,

        decimal WeightKg,
        decimal DeclaredValue,
        decimal? CashOnDeliveryAmount,
        string Description);
}
