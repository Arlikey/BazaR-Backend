using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Domain.Categories.Events;

using BazaR.Backend.Domain.Common;

public sealed record CategoryCreatedEvent(CategoryId CategoryId) : DomainEvent;

