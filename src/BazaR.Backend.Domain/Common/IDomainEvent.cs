using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
