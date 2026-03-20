using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Favorites.DTOs
{
    public sealed class FavoriteProductDto
    {
        public Guid ProductId { get; init; }
        public string Name { get; init; } = default!;
        public string Slug { get; init; } = default!;
        public decimal Price { get; init; }
        public decimal? OldPrice { get; init; }
        public string? MainImageUrl { get; init; }
        public bool IsAvailable { get; init; }
        public DateTime AddedAtUtc { get; init; }
    }
}
