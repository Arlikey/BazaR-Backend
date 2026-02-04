using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Sales;          
using BazaR.Backend.Domain.Users;
using BazaR.Backend.Domain.Catalog.Attributes;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Order> Orders => Set<Order>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
    public DbSet<Offer> Offers => Set<Offer>(); 


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
