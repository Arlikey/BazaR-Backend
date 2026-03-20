using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Abstractions.Services
{
    public interface ICheckoutSnapshotBuilder
    {
        Task<Result<IReadOnlyCollection<CheckoutLineSnapshotData>>> BuildAsync(
            Cart cart,
            CancellationToken ct);
    }
}
