using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Repositories.ReadModels;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Catalog.Products.Services;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Common.Abstractions.Security;
using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Infrastructure.Auth;
using BazaR.Backend.Infrastructure.Persistence;
using BazaR.Backend.Infrastructure.Persistence.Files;
using BazaR.Backend.Infrastructure.Persistence.ReadModels;
//using BazaR.Backend.Infrastructure.Persistence.Queries;
using BazaR.Backend.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BazaR.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Default"),
                npgsql =>
                {
                    // если миграции лежат в Infrastructure
                    npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);

                    
                    npgsql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });

           
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
        });


        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartReadRepository, CartReadRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAttributeDefinitionRepository, AttributeDefinitionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAttributeDefinitionReadRepository, AttributeDefinitionReadRepository>();
        services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IProductAttributesReadService, ProductAttributesReadService>();
        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<ISellerReadRepository, SellerReadRepository>();
        services.AddScoped<IAttributeUsageChecker, AttributeUsageChecker>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IOfferReadRepository, OfferReadRepository>();
        services.AddScoped<IProductOfferAttacher, ProductOfferAttacher>();
        services.AddScoped<IProductImageStorage, ProductImageStorage>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        return services;
    }
}