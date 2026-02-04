using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Catalog.Products;

namespace BazaR.Backend.Domain.Carts.Events;

public sealed record CartItemAddedEvent(CartId CartId, ProductId ProductId, int Quantity) : DomainEvent;