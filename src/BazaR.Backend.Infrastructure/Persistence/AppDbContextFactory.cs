using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO; 

namespace BazaR.Backend.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
       
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        //var cs = configuration.GetConnectionString("Default")
                 //?? "Host=localhost;Port=5432;Database=bazar;Username=bazar;Password=bazar_pass";

        var cs = configuration.GetConnectionString("Default")
                 ?? "Host=bazar-postgres-01.postgres.database.azure.com;Port=5432;Database=bazar;Username=bazaradmin;Password=qwerty!545455;SSL Mode=Require;Trust Server Certificate=true";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs, npgsql =>
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

        return new AppDbContext(optionsBuilder.Options);
    }
}