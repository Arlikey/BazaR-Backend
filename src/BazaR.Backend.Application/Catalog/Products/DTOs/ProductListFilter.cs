using BazaR.Backend.Domain.Catalog.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Catalog.Products.DTOs;

public sealed record ProductListFilter(
    Guid? SellerId = null,
    ProductStatus? Status = null,
    string? Search = null
);
