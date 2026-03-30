using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Favorites;
using BazaR.Backend.Domain.Identity;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.PaymentProfiles;
using BazaR.Backend.Domain.Payments;
using BazaR.Backend.Domain.Sales;          
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Shipping;
using BazaR.Backend.Domain.Shippings;
using BazaR.Backend.Domain.Users;
using BazaR.Backend.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Shipping> Shippings => Set<Shipping>();
    public DbSet<Checkout> Checkouts => Set<Checkout>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ShippingProfile> ShippingProfiles => Set<ShippingProfile>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<PaymentProfile> PaymentProfiles => Set<PaymentProfile>();
    public DbSet<PaymentMethodConfig> PaymentProfileMethods => Set<PaymentMethodConfig>();

    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<AuthUser> AuthUsers => Set<AuthUser>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Seller> Sellers => Set<Seller>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
