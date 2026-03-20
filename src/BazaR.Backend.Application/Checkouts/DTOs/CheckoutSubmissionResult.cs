using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Checkouts.DTOs
{
    public sealed record CheckoutSubmissionResult(
        IReadOnlyCollection<Guid> OrderIds,
        IReadOnlyCollection<Guid> ShippingIds,
        IReadOnlyCollection<Guid> PaymentIds);
}
