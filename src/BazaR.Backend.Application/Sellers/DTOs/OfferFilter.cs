using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Sellers.DTOs
{
    public class OfferFilter
    {
        public Guid? ProductId { get; set; }
        public Guid? SellerId { get; set; }
        public string? Status { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }
        public string? SearchSku { get; set; }
    }
}
