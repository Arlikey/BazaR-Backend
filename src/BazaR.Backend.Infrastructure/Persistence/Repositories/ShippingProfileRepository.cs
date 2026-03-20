using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Shipping;
using Microsoft.EntityFrameworkCore;
using System;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class ShippingProfileRepository : IShippingProfileRepository
{
    private readonly AppDbContext _db;

    public ShippingProfileRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ShippingProfile?> GetBySellerIdAsync(SellerId sellerId, CancellationToken ct)
    {
        return await _db.ShippingProfiles
            .Include(x => x.Methods)
            .FirstOrDefaultAsync(x => x.SellerId == sellerId, ct);
    }

    public async Task<ShippingProfile?> GetActiveBySellerIdAsync(SellerId sellerId, CancellationToken ct)
    {
        return await _db.ShippingProfiles
            .Include(x => x.Methods)
            .FirstOrDefaultAsync(
                x => x.SellerId == sellerId &&
                     x.Status == ShippingProfileStatus.Active,
                ct);
    }

    public void Add(ShippingProfile profile)
    {
        _db.ShippingProfiles.Add(profile);
    }

    public void Update(ShippingProfile profile)
    {
        _db.ShippingProfiles.Update(profile);
    }
}