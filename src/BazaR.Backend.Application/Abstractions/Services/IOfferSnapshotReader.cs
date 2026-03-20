using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Domain.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Abstractions.Services
{
    public interface IOfferSnapshotReader
    {
        Task<OfferSnapshotDto?> GetByIdAsync(OfferId offerId, CancellationToken ct);
    }
}
