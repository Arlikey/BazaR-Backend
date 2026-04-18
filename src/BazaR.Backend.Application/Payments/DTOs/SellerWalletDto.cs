using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Payments.DTOs
{
    public sealed class SellerWalletDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = default!;
    }
}
