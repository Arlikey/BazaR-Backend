using BazaR.Backend.Domain.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Domain.Users.Events;

public sealed record UserBlockedEvent(UserId UserId) : DomainEvent;
