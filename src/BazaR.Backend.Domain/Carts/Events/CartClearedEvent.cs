using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Carts.Events;

public sealed record CartClearedEvent(CartId CartId) : DomainEvent;