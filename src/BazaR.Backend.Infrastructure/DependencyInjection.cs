using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Abstractions.Integrations.NovaPoshta;

using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Repositories.ReadModels;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Catalog.Browsing.Abstractions;
using BazaR.Backend.Application.Catalog.Browsing.Sidebar.Abstractions;
using BazaR.Backend.Application.Catalog.Products.Services;
using BazaR.Backend.Application.Checkouts;
using BazaR.Backend.Application.Checkouts.Services;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Common.Abstractions.Security;
using BazaR.Backend.Application.Reviews.ProductReviews.Abstractions;
using BazaR.Backend.Application.Reviews.SellerReviews.Abstractions;
using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Domain.Reviews.ProductRatings;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using BazaR.Backend.Domain.Reviews.SellerRatings;
using BazaR.Backend.Domain.Reviews.SellerReviews;
using BazaR.Backend.Domain.Shippings;
using BazaR.Backend.Infrastructure.Auth;
using BazaR.Backend.Infrastructure.Integrations.NovaPoshta;

using BazaR.Backend.Infrastructure.Payments;
using BazaR.Backend.Infrastructure.Persistence;
using BazaR.Backend.Infrastructure.Persistence.Files;
using BazaR.Backend.Infrastructure.Persistence.ReadModels;
//using BazaR.Backend.Infrastructure.Persistence.Queries;
using BazaR.Backend.Infrastructure.Persistence.Repositories;
using BazaR.Backend.Infrastructure.Persistence.Services;
using BazaR.Backend.Infrastructure.Repositories;
using BazaR.Backend.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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


        services.AddOptions<LiqPayOptions>()
            .Bind(configuration.GetSection(LiqPayOptions.SectionName));

        // Регистрация HttpClient для платежного шлюза
        //services.AddHttpClient<IPaymentGateway, LiqPayPaymentGateway>();




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

        services.Configure<AzureBlobStorageOptions>(
        configuration.GetSection("BlobStorage"));

        services.AddScoped<IFileStorage, AzureBlobFileStorage>();

        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IBrandReadRepository, BrandReadRepository>();

        services.AddScoped<ICatalogBrowseReadRepository, CatalogBrowseReadRepository>();
        services.AddScoped<ICatalogSidebarReadRepository, CatalogSidebarReadRepository>();
        services.AddScoped<ProductOfferAttacher>();

        services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
        services.AddScoped<ISellerReviewRepository, SellerReviewRepository>();
        services.AddScoped<IProductRatingSummaryRepository, ProductRatingSummaryRepository>();
        services.AddScoped<ISellerRatingSummaryRepository, SellerRatingSummaryRepository>();
        services.AddScoped<IProductReviewRatingReader, ProductReviewRatingReader>();
        services.AddScoped<ISellerReviewRatingReader, SellerReviewRatingReader>();

        services.AddScoped<ProductRatingSummaryUpdater>();
        services.AddScoped<SellerRatingSummaryUpdater>();

        services.AddScoped<IProductReviewReadRepository, ProductReviewReadRepository>();
        services.AddScoped<ISellerReviewReadRepository, SellerReviewReadRepository>();

        services.AddScoped<IShippingRepository, ShippingRepository>();
        services.AddScoped<IShippingReadRepository, ShippingReadRepository>();
        services.AddScoped<ICheckoutRepository, CheckoutRepository>();
        services.AddScoped<IShippingProfileRepository, ShippingProfileRepository>();
        services.AddScoped<IShippingSelectionFactory, ShippingSelectionFactory>();
        services.AddScoped<ICheckoutSubmissionService, CheckoutSubmissionService>();
        services.AddScoped<IOrderFactory, OrderFactory>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentProfileRepository, PaymentProfileRepository>();

        services.AddScoped<ICheckoutSnapshotBuilder, CheckoutSnapshotBuilder>();
        services.AddScoped<IOfferSnapshotReader, OfferSnapshotReader>();

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ILiqPayCheckoutService, LiqPayCheckoutService>();

        // Регистрация настроек NovaPoshta
        services.Configure<NovaPoshtaOptions>(configuration.GetSection("NovaPoshta"));

        // Регистрация HTTP-клиента для Nova Poshta API
        services.AddHttpClient<INovaPoshtaGateway, NovaPoshtaGateway>();



       /* services.Configure<NovaPostSandboxOptions>(configuration.GetSection("NovaPostSandbox"));

        services.AddHttpClient<INovaPostSandboxTokenProvider, NovaPostSandboxTokenProvider>();

       
        services.AddHttpClient<INovaPostDirectoryGateway, NovaPostDirectoryGateway>();
        services.AddHttpClient<INovaPostShipmentGateway, NovaPostShipmentGateway>();

        

        services.AddScoped<IShipmentCreationStrategy, CashOnDeliveryShipmentStrategy>();
        services.AddScoped<IShipmentCreationStrategy, AfterPaymentShipmentStrategy>();
        services.AddScoped<IShipmentStrategyFactory, ShipmentStrategyFactory>();*/

        return services;
    }
}