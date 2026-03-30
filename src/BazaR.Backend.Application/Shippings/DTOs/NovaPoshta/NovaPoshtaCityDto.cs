using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Shippings.DTOs.NovaPoshta
{
    public sealed record NovaPoshtaCityDto(
    string Ref,
    string Description,
    string? Area,
    string? SettlementType);
}
